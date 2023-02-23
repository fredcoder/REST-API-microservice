using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using WebApplication.Core.Common.Models;
using WebApplication.Core.Users.Commands;
using WebApplication.Core.Users.Common.Models;
using WebApplication.IntegrationTests.Extensions;
using Xunit;

namespace WebApplication.IntegrationTests
{
    public class UsersControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly IFixture _fixture;
        private readonly WebApplicationFactory<Startup> _factory;

        public UsersControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _fixture = new Fixture();
        }

        // TEST NAME - getUser
        // TEST DESCRIPTION - Get user should succeed
        [Fact]
        public async Task GivenUserId1_WhenGettingUsers_ThenReturnLauraBailey()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var url = $"{client.BaseAddress}Users?id=1";

            // Act
            HttpResponseMessage responseMessage = await client.GetAsync(url);

            // Assert
            responseMessage.EnsureSuccessStatusCode();
            var user = await responseMessage.DeserializeContentAsync<UserDto>();
            user.GivenNames.Should()
                .Be("Laura");

            user.LastName.Should()
                .Be("Bailey");

            user.EmailAddress.Should()
                .BeNull();

            user.ToString()
                .Should()
                .Be("Laura Bailey");
        }

        // TEST NAME - getUser
        // TEST DESCRIPTION - Invalid user id should return bad request
        [Theory]
        [AutoData]
        public async Task GivenInvalidUserId_WhenGettingUserById_ThenReturnBadRequest([Range(-99, 0)]int userId)
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var url = $"{client.BaseAddress}Users?id={userId}";

            // Act
            HttpResponseMessage responseMessage = await client.GetAsync(url);

            // Assert
            responseMessage.StatusCode.Should()
                           .Be(StatusCodes.Status400BadRequest);

            var problemDetails = await responseMessage.DeserializeContentAsync<StatusCodeProblemDetails>();
            problemDetails.Detail.Should()
                          .Be("'Id' must be greater than '0'.");
        }

        // TEST NAME - getUser
        // TEST DESCRIPTION - Out of range user id should return not found
        [Theory]
        [AutoData]
        public async Task GivenOutOfRangeUserId_WhenGettingUserById_ThenReturnNotFound([Range(20,99)]int userId)
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var url = $"{client.BaseAddress}Users?id={userId}";

            // Act
            HttpResponseMessage responseMessage = await client.GetAsync(url);

            // Assert
            responseMessage.StatusCode.Should()
                           .Be(StatusCodes.Status404NotFound);

            var problemDetails = await responseMessage.DeserializeContentAsync<StatusCodeProblemDetails>();
            problemDetails.Detail.Should()
                          .Be($"The user '{userId}' could not be found.");
        }

        // TEST NAME - findUsers
        // TEST DESCRIPTION - Find users should succeed
        [Fact]
        public async Task GivenUnpopulatedUser_WhenUpdatingUser_ThenReturnBadRequest()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var url = $"{client.BaseAddress}Users";
            var command = new UpdateUserCommand();
            string stringContent = JsonConvert.SerializeObject(command);

            // Act
            HttpResponseMessage responseMessage = await client.PutAsync(url, new StringContent(stringContent, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            responseMessage.StatusCode.Should()
                           .Be(StatusCodes.Status400BadRequest);

            var problemDetails = await responseMessage.DeserializeContentAsync<StatusCodeProblemDetails>();
            problemDetails.Detail.Should()
                          .Be("'Id' must be greater than '0'.;'Given Names' must not be empty.;'Last Name' must not be empty.;'Email Address' must not be empty.;'Mobile Number' must not be empty.");
        }

        [Theory]
        [AutoData]
        public async Task GivenPageNumberLessThanOrEqualToZero_WhenListingUsers_ThenReturnBadRequest(
            [Range(-99, 0)] int pageNumber)
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var url = $"{client.BaseAddress}Users/List?pageNumber={pageNumber}";

            // Act
            HttpResponseMessage responseMessage = await client.GetAsync(url);

            // Assert
            responseMessage.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var problemDetails = await responseMessage.DeserializeContentAsync<StatusCodeProblemDetails>();
            problemDetails.Detail.Should().Be("'Page Number' must be greater than '0'.");
        }

        [Fact]
        public async Task GivenLastNameSmith_WhenGettingUsers_ThenReturnJohnAndJaneSmith()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var url = $"{client.BaseAddress}Users/Find?lastName=Smith";
            IEnumerable<UserDto> expectedUsers = new List<UserDto>
            {
                new UserDto
                {
                    UserId = 10,
                    GivenNames = "John",
                    LastName = "Smith",
                    EmailAddress = "john.smith@example.com",
                    MobileNumber = "0411112222",
                },
                new UserDto
                {
                    UserId = 11,
                    GivenNames = "Jane",
                    LastName = "Smith",
                    EmailAddress = "jane.smith@example.com",
                    MobileNumber = "0422223333",
                }
            };

            // Act
            HttpResponseMessage responseMessage = await client.GetAsync(url);

            // Assert
            responseMessage.EnsureSuccessStatusCode();
            var users = await responseMessage.DeserializeContentAsync<IEnumerable<UserDto>>();
            List<UserDto> actualUsers = users.ToList();
            actualUsers.Count
                       .Should()
                       .Be(2);

            actualUsers.Should().BeEquivalentTo(expectedUsers);
        }

        // TEST NAME - listUser
        // TEST DESCRIPTION - Listing users on page 1 should succeed and has next page should be true
        [Fact]
        public async Task GivenPageNumber1_WhenGettingListOfUsers_ThenReturnFirstTenUsers_AndHasNextPageShouldBeTrue()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var url = $"{client.BaseAddress}Users/List?pageNumber=1";

            // ACT
            HttpResponseMessage responseMessage = await client.GetAsync(url);

            // Assert
            responseMessage.EnsureSuccessStatusCode();
            var users = await responseMessage.DeserializeContentAsync<PaginatedDto<IEnumerable<UserDto>>>();
            users.HasNextPage.Should()
                 .BeTrue();

            users.Data.Count()
                 .Should()
                 .Be(10);
        }

        // TEST NAME - listUser
        // TEST DESCRIPTION - Listing users on page 2 should succeed with has next page as false
        [Fact]
        public async Task GivenPageNumber2_WhenGettingListOfUsers_ThenReturnPage2OfUsers_AndHasNextPageShouldBeFalse()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var url = $"{client.BaseAddress}Users/List?pageNumber=2";

            // ACT
            HttpResponseMessage responseMessage = await client.GetAsync(url);

            // Assert
            responseMessage.EnsureSuccessStatusCode();
            var users = await responseMessage.DeserializeContentAsync<PaginatedDto<IEnumerable<UserDto>>>();
            users.HasNextPage.Should()
                 .BeFalse();

            users.Data.Count()
                 .Should()
                 .BeGreaterOrEqualTo(1);
        }

        // TEST NAME - listUser
        // TEST DESCRIPTION - Listing 5 users should return only 5 users
        [Fact]
        public async Task GivenFiveItemsPerPage_WhenGettingListOfUsers_ThenReturnFiveUsers()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var url = $"{client.BaseAddress}Users/List?pageNumber=1&itemsPerPage=5";

            // ACT
            HttpResponseMessage responseMessage = await client.GetAsync(url);

            // Assert
            responseMessage.EnsureSuccessStatusCode();
            var users = await responseMessage.DeserializeContentAsync<PaginatedDto<IEnumerable<UserDto>>>();
            users.HasNextPage.Should()
                 .BeTrue();

            users.Data.Count()
                 .Should()
                 .Be(5);
        }

        // TEST NAME - createUser
        // TEST DESCRIPTION - Create user should succeed
        [Fact]
        public async Task GivenValidUser_WhenCreatingNewUser_ThenReturnCreatedUser()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var url = $"{client.BaseAddress}Users";
            var command = _fixture.Create<CreateUserCommand>();
            string stringContent = JsonConvert.SerializeObject(command);

            // Act
            HttpResponseMessage responseMessage = await client.PostAsync(url, new StringContent(stringContent, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            responseMessage.EnsureSuccessStatusCode();
            var createdUser = await responseMessage.DeserializeContentAsync<UserDto>();
            responseMessage.Headers.Location.Should()
                           .Be($"/Users?id={createdUser.UserId}");

            var getUrl = $"{client.BaseAddress}Users?id={createdUser.UserId}";
            HttpResponseMessage getResponse = await client.GetAsync(getUrl);
            getResponse.EnsureSuccessStatusCode();

            var storedUser = await getResponse.DeserializeContentAsync<UserDto>();
            storedUser.Should().BeEquivalentTo(createdUser);

            createdUser.UserId.Should()
                       .BeGreaterThan(0);

            createdUser.GivenNames.Should()
                       .Be(command.GivenNames);

            createdUser.LastName.Should()
                       .Be(command.LastName);

            createdUser.EmailAddress.Should()
                       .Be(command.EmailAddress);

            createdUser.MobileNumber.Should()
                       .Be(command.MobileNumber);
        }

        // TEST NAME - createUser
        // TEST DESCRIPTION - Create user should fail for missing properties
        [Theory]
        [InlineData("", "", "", "")]
        [InlineData("Jake", "", "", "")]
        [InlineData("Jake", "Brian", "", "")]
        [InlineData("Jake", "Brian", "jake.brian@example.com", "")]
        public async Task GivenInvalidUser_WhenCreatingNewUser_ThenReturnBadRequest(
            string givenNames,
            string lastName,
            string email,
            string mobile
        )
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var url = $"{client.BaseAddress}Users";
            var command = new CreateUserCommand
            {
                GivenNames = givenNames,
                LastName = lastName,
                EmailAddress = email,
                MobileNumber = mobile
            };
            string stringContent = JsonConvert.SerializeObject(command);

            // Act
            HttpResponseMessage responseMessage = await client.PostAsync(url, new StringContent(stringContent, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            responseMessage.StatusCode.Should()
                           .Be(StatusCodes.Status400BadRequest);

            var problemDetails = await responseMessage.DeserializeContentAsync<StatusCodeProblemDetails>();
            problemDetails.Detail.Should()
                          .NotBeEmpty();
        }

        // TEST NAME - deleteUser
        // TEST DESCRIPTION - Deleting user should succeed
        [Fact]
        public async Task GivenValidUserId_WhenDeletingUser_ThenReturnDeletedUserDetails()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var url = $"{client.BaseAddress}Users?id=1";
            HttpResponseMessage getResponse = await client.GetAsync(url);
            var expected = await getResponse.DeserializeContentAsync<UserDto>();

            // Act
            HttpResponseMessage responseMessage = await client.DeleteAsync(url);

            // Assert
            responseMessage.EnsureSuccessStatusCode();
            var deletedUser = await responseMessage.DeserializeContentAsync<UserDto>();
            deletedUser.Should().BeEquivalentTo(expected);
        }

        // TEST NAME - deleteUser
        // TEST DESCRIPTION - Deleting user should fail with bad request when user id is invalid
        [Theory]
        [AutoData]
        public async Task GivenInvalidUserId_WhenDeletingUser_ThenReturnBadRequest([Range(-99, 0)] int userId)
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var url = $"{client.BaseAddress}Users?id={userId}";

            // Act
            HttpResponseMessage responseMessage = await client.DeleteAsync(url);

            // Assert
            responseMessage.StatusCode.Should()
                           .Be(StatusCodes.Status400BadRequest);

            var problemDetails = await responseMessage.DeserializeContentAsync<StatusCodeProblemDetails>();
            problemDetails.Detail.Should()
                          .Be("'Id' must be greater than '0'.");
        }

        // TEST NAME - deleteUser
        // TEST DESCRIPTION - Deleting user should fail with not found if user id is out of range
        [Theory]
        [AutoData]
        public async Task GivenOutOfRangeUserId_WhenDeletingUser_ThenReturnNotFound([Range(20, 99)] int userId)
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var url = $"{client.BaseAddress}Users?id={userId}";

            // Act
            HttpResponseMessage responseMessage = await client.DeleteAsync(url);

            // Assert
            responseMessage.StatusCode.Should()
                           .Be(StatusCodes.Status404NotFound);

            var problemDetails = await responseMessage.DeserializeContentAsync<StatusCodeProblemDetails>();
            problemDetails.Detail.Should()
                          .Be($"The user '{userId}' could not be found.");
        }

        // TEST NAME - updateUser
        // TEST DESCRIPTION - Updating user should succeed
        [Fact]
        public async Task GivenUserNotInDb_WhenUpdatingUser_ThenReturnNotFound()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var url = $"{client.BaseAddress}Users";
            UpdateUserCommand command = _fixture.Build<UpdateUserCommand>()
                                                .With(x => x.Id, 200)
                                                .Create();

            string stringContent = JsonConvert.SerializeObject(command);

            // Act
            HttpResponseMessage responseMessage = await client.PutAsync(url, new StringContent(stringContent, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            responseMessage.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            var problemDetails = await responseMessage.DeserializeContentAsync<StatusCodeProblemDetails>();
            problemDetails.Detail.Should().Be("The user '200' could not be found.");
        }

        [Fact]
        public async Task GivenExistingUser_WhenUpdatingUser_ThenReturnUpdatedUser()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var url = $"{client.BaseAddress}Users";
            UpdateUserCommand command = _fixture.Build<UpdateUserCommand>()
                                                .With(x => x.Id, 1)
                                                .Create();

            string stringContent = JsonConvert.SerializeObject(command);

            // Act
            HttpResponseMessage responseMessage = await client.PutAsync(url, new StringContent(stringContent, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            responseMessage.EnsureSuccessStatusCode();
            var updatedUser = await responseMessage.DeserializeContentAsync<UserDto>();

            var getUrl = $"{client.BaseAddress}Users?id=1";
            HttpResponseMessage getResponse = await client.GetAsync(getUrl);
            getResponse.EnsureSuccessStatusCode();
            var getUser = await getResponse.DeserializeContentAsync<UserDto>();
            getUser.Should().BeEquivalentTo(updatedUser);

            updatedUser.UserId.Should()
                       .Be(1);

            updatedUser.GivenNames.Should()
                       .Be(command.GivenNames);

            updatedUser.LastName.Should()
                       .Be(command.LastName);

            updatedUser.EmailAddress.Should()
                       .Be(command.EmailAddress);

            updatedUser.MobileNumber.Should()
                       .Be(command.MobileNumber);
        }
    }
}
