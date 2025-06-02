
using FiapCloud.Infra.Identity;
using FiapCLoud.Domain.Entities;
using FiapCLoud.Domain.Interfaces;
using FiapCLoud.Domain.Services;
using FIapCloud.App.Dtos;
using Microsoft.AspNetCore.Identity;
using FiapCLoud.Domain.Exceptions;
using System.Net;
using Microsoft.Extensions.Logging;


namespace FIapCloud.App.Services;

public class UserAppService : IUserAppService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserRepository _userRepository;
    private readonly IUserDomainService _userDomainService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<UserAppService> _logger;

    public UserAppService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IUserRepository userRepository,
        IUserDomainService userDomainService,
        ITokenService tokenService,
        ILogger<UserAppService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userRepository = userRepository;
        _userDomainService = userDomainService;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Guid> CreateUserAsync(CreateUserRequest request)
    {
        await _userDomainService.EnsureEmailIsUniqueAsync(request.Email);

        var appUser = new ApplicationUser { UserName = request.Email, Email = request.Email };
        var identityResult = await _userManager.CreateAsync(appUser, request.Password);

        if (!identityResult.Succeeded)
        {
            var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
            _logger.LogWarning("AppService: Falha ao criar ApplicationUser {UserEmail}. Erros: {IdentityErrors}", request.Email, errors);
            throw new DomainException(errors, HttpStatusCode.BadRequest);
        }

        var roleResult = await _userManager.AddToRoleAsync(appUser, "Usuario");
        if (!roleResult.Succeeded)
        {
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            _logger.LogError("AppService: Falha ao adicionar AppUser {AppUserId} ao papel 'Usuario'. Erros: {IdentityErrors}", appUser.Id, errors);
        
            throw new DomainException($"Falha ao atribuir papel ao usuário: {errors}", HttpStatusCode.InternalServerError);
        }

        var domainUser = new DomainUser(request.Name, appUser.Id);
        await _userRepository.AddAsync(domainUser);
        
        await _userRepository.CommitAsync();

        _logger.LogInformation("AppService: Usuário {UserEmail} (DomainUserID: {DomainUserId}, AppUserID: {AppUserId}) criado com sucesso.", request.Email, domainUser.Id, appUser.Id);
        return domainUser.Id;
    }

    public async Task<LoginUserResponse> LoginUserAsync(LoginUserRequest loginRequest)
    {
        var appUser = await _userManager.FindByEmailAsync(loginRequest.Email);
        if (appUser == null)
        {
            _logger.LogWarning("AppService: Login falhou - usuário {UserEmail} não encontrado.", loginRequest.Email);
            throw new DomainException("Usuário ou senha inválidos.", HttpStatusCode.Unauthorized);
        }

        var domainUser = await _userRepository.GetByApplicationUserIdAsync(appUser.Id);
        if (domainUser != null && !domainUser.IsActive)
        {
            _logger.LogWarning("AppService: Login falhou - usuário {UserEmail} (AppUserID: {AppUserId}) está desativado.", appUser.Email, appUser.Id);
            throw new DomainException("Usuário desativado. Entre em contato com o suporte.", HttpStatusCode.Forbidden);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(appUser, loginRequest.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            _logger.LogWarning("AppService: Login falhou - usuário {UserEmail} (AppUserID: {AppUserId}) bloqueado.", appUser.Email, appUser.Id);
            throw new DomainException("Conta bloqueada por excesso de tentativas.", HttpStatusCode.Forbidden);
        }
        if (!result.Succeeded)
        {
            _logger.LogWarning("AppService: Login falhou - senha inválida para {UserEmail} (AppUserID: {AppUserId}).", appUser.Email, appUser.Id);
            throw new DomainException("Usuário ou senha inválidos.", HttpStatusCode.Unauthorized);
        }

        _logger.LogInformation("AppService: Login bem-sucedido para {UserEmail} (AppUserID: {AppUserId}).", appUser.Email, appUser.Id);
        return await _tokenService.GenerateToken(appUser);
    }

    public async Task<Guid> ChangeUserStatusAsync(Guid applicationUserId, ChangeUserStatusRequest changeUserStatusRequest)
    {
        var domainUser = await _userRepository.GetByApplicationUserIdAsync(applicationUserId);
        if (domainUser == null)
        {
            throw new DomainException($"Usuário não encontrado para alteração de status (AppUserID: {applicationUserId}).", HttpStatusCode.NotFound);
        }

        _logger.LogInformation("AppService: Processando alteração de status para AppUserID {ApplicationUserId} (DomainUserID: {DomainUserId}) para IsActive: {IsActive}.", applicationUserId, domainUser.Id, changeUserStatusRequest.IsActive);
        if (changeUserStatusRequest.IsActive)
        {
            await _userDomainService.ActivateUserAsync(domainUser.Id);
        }
        else
        {
            await _userDomainService.DeactivateUserAsync(domainUser.Id);
        }
        return applicationUserId;
    }
}