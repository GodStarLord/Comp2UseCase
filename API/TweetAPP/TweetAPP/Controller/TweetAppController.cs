namespace TweetAPP.Controller
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using TweetAPP.Models;
    using TweetAPP.Service;

    [Route("api/v1.0/tweets/")]
    [ApiController]
    public class TweetAppController : ControllerBase
    {
        private readonly ITweetAppService tweetAppService;
        private readonly IConfiguration configuration;
        private ILogger<TweetAppController> logger;

        public TweetAppController(ITweetAppService tweetAppService, ILogger<TweetAppController> logger, IConfiguration configuration)
        {
            this.tweetAppService = tweetAppService;
            this.configuration = configuration;
            this.logger = logger;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            try
            {
                var result = await this.tweetAppService.Register(user);
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while registering user");
                throw;
            }
        }

        [HttpGet]
        [Route("login/{username},{password}")]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                Token token = null;
                var result = await this.tweetAppService.UserLogin(username, password);
                if (result != null)
                {
                    token = new Token() { UserId = result.UserId, Username = result.Username, Tokens = this.GenerateJwtToken(username), Message = "Success" };
                }
                else
                {
                    token = new Token() { Tokens = null, Message = "UnSuccess" };
                }

                return this.Ok(token);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while login user");
                throw;
            }
        }

        [HttpPost]
        [Route("tweet")]
        public async Task<IActionResult> Tweet(Tweet tweet)
        {
            try
            {
                var result = await this.tweetAppService.PostTweet(tweet);
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while posting user tweet");
                throw;
            }
        }

        [HttpDelete]
        [Route("tweetdelete/{username},{tweet}")]
        public async Task<IActionResult> DeleteTweet(string username, string tweet)
        {
            try
            {
                var result = await this.tweetAppService.DeleteTweet(username, tweet);
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while Deleteing user tweet");
                throw;
            }
        }

        [HttpGet]
        [Route("users/all")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var result = await this.tweetAppService.GetAllUsers();
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while retrieving users");
                throw;
            }
        }

        [HttpGet]
        [Route("user/search/{username}")]
        public async Task<IActionResult> GetTweetsByUser(string username)
        {
            try
            {
                var result = await this.tweetAppService.GetTweetsByUser(username);
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while fetching tweets by user");
                throw;
            }
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllTweets()
        {
            try
            {
                var result = await this.tweetAppService.GetAllTweets();
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while fetching user tweets");
                throw;
            }
        }

        [HttpGet]
        [Route("allcomments/{username},{tweet}")]
        public async Task<IActionResult> GetAllComments(string username, string tweet)
        {
            try
            {
                var result = await this.tweetAppService.GetComments(username, tweet);
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while fetching user comments");
                throw;
            }
        }

        [HttpGet]
        [Route("user/{username}")]
        public async Task<IActionResult> GetUserProfile(string username)
        
       {
            try
            {
                var result = await this.tweetAppService.GetUserProfile(username);
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while fetching user");
                throw;
            }
        }

        [HttpPut]
        [Route("update/{emailId},{oldpassword},{newpassword}")]
        public async Task<IActionResult> UpdatePassword(string emailId, string oldpassword, string newPassword)
        {
            try
            {
                var result = await this.tweetAppService.UpdatePassword(emailId, oldpassword, newPassword);
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while updating user password");
                throw;
            }
        }

        [HttpPut]
        [Route("forgot/{emailId},{password}")]
        public async Task<IActionResult> ForgotPassword(string emailId, string password)
        {
            try
            {
                var result = await this.tweetAppService.ForgotPassword(emailId, password);
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while reseting user password");
                throw;
            }
        }


        [HttpPost]
        [Route("reply/{comment},{username},{Name},{tweet}")]
        public async Task<IActionResult> PostComment(string comment, string username, string Name, string tweet)
        {
            try
            {
                var result = await this.tweetAppService.Comments(comment, username,Name, tweet);
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while posting user comment");
                throw;
            }
        }


        [HttpGet]
        [Route("likes/{username},{tweet}")]
        public async Task<IActionResult> GetLikes(string username, string tweet)
        {
            try
            {
                var result = await this.tweetAppService.Likes(username, tweet);
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while getting user like");
                throw;
            }
        }

        private string GenerateJwtToken(string emailId)
        {
            // 1. Create Security Token Handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // 2. Create Private Key to Encrypted
            var tokenKey = Encoding.ASCII.GetBytes(configuration["AppSettings:Token"]);

            //3. Create JETdescriptor
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, emailId)
                    }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            //4. Create Token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // 5. Return Token from method
            return tokenHandler.WriteToken(token);
        }
    }
}
