using AutoMapper;
using EcommerceBackend.Application.Dto.Auth;
using EcommerceBackend.Application.Interfaces;
using EcommerceBackend.Application.Interfaces.Services;
using EcommerceBackend.Domain.Models;
using System.Security.Claims;
using EcommerceBackend.Application.Exceptions;
using EcommerceBackend.Application.Exceptions.Token;
using EcommerceBackend.Application.Interfaces.Email;

namespace EcommerceBackend.Application.Services.Auth
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _user;
        private readonly ICartRepository _cart;
        private readonly IRoleRepository _role;
        private readonly IUserRoleRepository _userRole;
        private readonly IRefreshTokenRepository _refreshToken;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoginNotificationService _loginNotificationService;

        public AccountService(IUserRepository user, ICartRepository cart, IRoleRepository role, IUserRoleRepository userRole, IRefreshTokenRepository refreshToken, ITokenService tokenService, IMapper mapper, IUnitOfWork unitOfWork, ILoginNotificationService loginNotificationService)
        {
            _user = user;
            _cart = cart;
            _role = role;
            _userRole = userRole;
            _refreshToken = refreshToken;
            _tokenService = tokenService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _loginNotificationService = loginNotificationService;
        }

        public async Task<TokenDto> Login(LoginDto dto)
        {
            var user = await _user.GetByUsernameOrEmailAsync(dto.Account);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.HashPassword))
                throw new UnauthorizedException("Invalid username or password");
            if (!user.IsActive)
                throw new ForbiddenException("Account is deactivated");
            var claims = await BuildClaimsAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshEntity = new RefreshToken()
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = _tokenService.GetRefreshTokenExpiry(),
                IsRevoked = false,
                IsUsed = false
            };
            _refreshToken.Add(refreshEntity);
            await _unitOfWork.SaveChangesAsync();
            await _loginNotificationService.NotifyLoginSuccessAsync(user);
            return new TokenDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task Logout(RefreshTokenDto dto)
        {
            var token = await _refreshToken.GetByTokenAsync(dto.RefreshToken);
            if (token != null)
            {
                token.IsRevoked = true;
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<TokenDto> RefreshToken(RefreshTokenDto dto)
        {
            var oldRefreshTokenEntity = await _refreshToken.GetByTokenWithUserAsync(dto.RefreshToken);
            if (oldRefreshTokenEntity == null)
                throw new InvalidTokenException("Invalid refresh token");
            if (oldRefreshTokenEntity.IsUsed)
            {
                await _tokenService.RevokeAllUserTokens(oldRefreshTokenEntity.UserId);
                throw new TokenReuseException("Token reuse detected");
            }
            if (oldRefreshTokenEntity.ExpiresAt < DateTime.UtcNow)
                throw new ExpiredTokenException("Refresh token expired");
            //logout,change password, admin revoke,revoke all user
            if (oldRefreshTokenEntity.IsRevoked)
                throw new InvalidTokenException("Refresh token revoked");
            oldRefreshTokenEntity.IsRevoked = true;
            oldRefreshTokenEntity.IsUsed = true;
            var user = oldRefreshTokenEntity.User;
            var claims = await BuildClaimsAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newRefreshTokenEntity = new RefreshToken()
            {
                Token = newRefreshToken,
                UserId = oldRefreshTokenEntity.UserId,
                ExpiresAt = _tokenService.GetRefreshTokenExpiry(),
                IsRevoked = false,
                IsUsed = false
            };
            _refreshToken.Add(newRefreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();
            return new TokenDto()
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task Register(RegisterDto dto)
        {
            if (await _user.ExistsByEmailAsync(dto.Email))
            {
                throw new AlreadyExistsException("Email already existed");
            }
            if (await _user.ExistsByUsernameAsync(dto.Username))
            {
                throw new AlreadyExistsException("Username already existed");
            }
            var user = _mapper.Map<User>(dto);
            user.HashPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            _user.Add(user);
            var cart = new Cart()
            {
                User = user
            };
            _cart.Add(cart);
            var customerRole = await _role.GetByNameAsync("CUSTOMER");
            if (customerRole == null)
            {
                throw new NotFoundException("CUSTOMER role not found");
            }
            var userRole = new UserRole()
            {
                User = user,
                RoleId = customerRole.Id
            };
            _userRole.Add(userRole);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<List<Claim>> BuildClaimsAsync(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.Email,user.Email)
            };
            var roles = await _userRole.GetRoleNameByUserAsync(user.Id);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }
    }
}
