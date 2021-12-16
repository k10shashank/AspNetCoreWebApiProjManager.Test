using AspNetCoreWebApiProjManager.Test.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreWebApiProjManager.Test
{
    public class BasicTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly string apiUrl = "api";
        private readonly string notPresentText = "not present.";
        private readonly string isAlreadyPresentText = "ID already present.";
        private readonly string isRequiredText = "is Required.";

        public BasicTests(WebApplicationFactory<Startup> fixture)
        {
            _client = fixture.CreateClient();
        }

        [Theory]
        [InlineData("Project")]
        [InlineData("Task")]
        [InlineData("User")]
        public async Task GetEndPointTest(string route)
        {
            // Arrange
            string url = $"{apiUrl}/{route}";

            // Act
            HttpResponseMessage response = await _client.GetAsync(url);
            HttpStatusCode statusCode = response.StatusCode;

            // Assert
            Assert.Equal(HttpStatusCode.OK, statusCode);
        }

        [Theory]
        [InlineData("Project", 1)]
        [InlineData("Task", 1)]
        [InlineData("User", 1)]
        [InlineData("Project", 2)]
        [InlineData("Task", 2)]
        [InlineData("User", 2)]
        [InlineData("Project", 3)]
        [InlineData("Task", 3)]
        [InlineData("User", 3)]
        [InlineData("Project", 4)]
        [InlineData("Task", 4)]
        [InlineData("User", 4)]
        [InlineData("Project", 5)]
        [InlineData("Task", 5)]
        [InlineData("User", 5)]
        [InlineData("Project", 6)]
        [InlineData("Task", 6)]
        [InlineData("User", 6)]
        [InlineData("Project", 7)]
        [InlineData("Task", 7)]
        [InlineData("User", 7)]
        [InlineData("Project", 8)]
        [InlineData("Task", 8)]
        [InlineData("User", 8)]
        [InlineData("Project", 9)]
        [InlineData("Task", 9)]
        [InlineData("User", 9)]
        public async Task GetByIdEndPointTest(string route, int id)
        {
            // Arrange
            string url = $"{apiUrl}/{route}/{id}";

            // Act
            HttpResponseMessage response = await _client.GetAsync(url);
            HttpStatusCode statusCode = response.StatusCode;
            string stringBody = await response.Content.ReadAsStringAsync();

            // Assert
            switch (statusCode)
            {
                case HttpStatusCode.OK:
                    int projectId = JsonConvert.DeserializeObject<ProjectModel>(stringBody).ID_PROJECT;
                    int taskId = JsonConvert.DeserializeObject<TaskModel>(stringBody).ID_TASK;
                    int userId = JsonConvert.DeserializeObject<UserModel>(stringBody).ID_USER;
                    Assert.True((userId == id && projectId == 0 && taskId == 0) || (userId == 0 && projectId == id && taskId == 0) || (userId != 0 && projectId != 0 && taskId == id));
                    break;
                case HttpStatusCode.NotFound:
                    ErrorModel error = JsonConvert.DeserializeObject<ErrorModel>(stringBody);
                    Assert.Equal(HttpStatusCode.NotFound, error.ERROR_CODE);
                    Assert.Equal($"{route} not present.", error.ERROR_MSG);
                    break;
                default:
                    Assert.True(false);
                    break;
            }
        }

        [Theory]
        [InlineData("Project")]
        [InlineData("Task")]
        [InlineData("User")]
        public async Task GetByIdEndPointLoopTest(string route)
        {
            for (int id = 1; id < 10; id++)
            {
                // Arrange
                string url = $"{apiUrl}/{route}/{id}";

                // Act
                HttpResponseMessage response = await _client.GetAsync(url);
                HttpStatusCode statusCode = response.StatusCode;
                string stringBody = await response.Content.ReadAsStringAsync();

                // Assert
                switch (statusCode)
                {
                    case HttpStatusCode.OK:
                        int projectId = JsonConvert.DeserializeObject<ProjectModel>(stringBody).ID_PROJECT;
                        int taskId = JsonConvert.DeserializeObject<TaskModel>(stringBody).ID_TASK;
                        int userId = JsonConvert.DeserializeObject<UserModel>(stringBody).ID_USER;
                        Assert.True((userId == id && projectId == 0 && taskId == 0) || (userId == 0 && projectId == id && taskId == 0) || (userId != 0 && projectId != 0 && taskId == id));
                        break;
                    case HttpStatusCode.NotFound:
                        ErrorModel error = JsonConvert.DeserializeObject<ErrorModel>(stringBody);
                        Assert.Equal(HttpStatusCode.NotFound, error.ERROR_CODE);
                        Assert.Equal($"{route} not present.", error.ERROR_MSG);
                        break;
                    default:
                        Assert.True(false);
                        break;
                }
            }
        }

        [Theory]
        [InlineData(1, "Test", "User", "test.user@test.com")]
        [InlineData(11, "Test", "User", "test.user@test.com")]
        [InlineData(11, null, "User", "test.user@test.com")]
        [InlineData(11, "Test", null, "test.user@test.com")]
        [InlineData(11, "Test", "User", null)]
        [InlineData(11, null, null, "test.user@test.com")]
        [InlineData(11, "Test", null, null)]
        [InlineData(11, null, "User", null)]
        [InlineData(11, null, null, null)]
        public async Task UserControllerTest(int id, string firstName, string lastName, string email)
        {
            // Common
            string route = "User";
            string url = $"{apiUrl}/{route}";
            ErrorModel error;

            // Get All
            // Act
            HttpResponseMessage initialGetResponse = await _client.GetAsync(url);
            HttpStatusCode initialGetStatusCode = initialGetResponse.StatusCode;
            string initialGetContentString = await initialGetResponse.Content.ReadAsStringAsync();
            IEnumerable<UserModel> initialGetContent = JsonConvert.DeserializeObject<IEnumerable<UserModel>>(initialGetContentString);
            int initialGetContentCount = initialGetContent.Count();

            // Assert
            Assert.Equal(HttpStatusCode.OK, initialGetStatusCode);


            // Get By Id
            // Act
            HttpResponseMessage getByIdResponse = await _client.GetAsync($"{url}/{id}");
            HttpStatusCode getByIdStatusCode = getByIdResponse.StatusCode;
            string getByIdContentString = await getByIdResponse.Content.ReadAsStringAsync();

            // Assert
            switch (getByIdStatusCode)
            {
                case HttpStatusCode.OK:
                    int userId = JsonConvert.DeserializeObject<UserModel>(getByIdContentString).ID_USER;
                    Assert.Equal(id, userId);
                    break;
                case HttpStatusCode.NotFound:
                    error = JsonConvert.DeserializeObject<ErrorModel>(getByIdContentString);
                    Assert.Equal(HttpStatusCode.NotFound, error.ERROR_CODE);
                    Assert.Equal($"{route} not present.", error.ERROR_MSG);
                    break;
                default:
                    Assert.True(false);
                    break;
            }


            // Post
            // Arrange
            UserModel userModel = new UserModel() { ID_USER = id, FIRST_NAME = firstName, LAST_NAME = lastName, EMAIL = email };
            HttpContent postContent = new StringContent(JsonConvert.SerializeObject(userModel), Encoding.UTF8, "application/json");
            string nullField = null;
            if (string.IsNullOrEmpty(email))
                nullField = "Email";
            else if (string.IsNullOrEmpty(firstName))
                nullField = "FirstName";
            else if (string.IsNullOrEmpty(lastName))
                nullField = "LastName";

            // Act
            HttpResponseMessage postReponse = await _client.PostAsync(url, postContent);
            HttpStatusCode postStatusCode = postReponse.StatusCode;
            string postGetContentString = await postReponse.Content.ReadAsStringAsync();

            // Assert
            switch (postStatusCode)
            {
                case HttpStatusCode.NoContent:
                    // Get All
                    // Act
                    HttpResponseMessage afterAddGetResponse = await _client.GetAsync(url);
                    HttpStatusCode afterAddGetStatusCode = afterAddGetResponse.StatusCode;
                    string afterAddGetContentString = await afterAddGetResponse.Content.ReadAsStringAsync();
                    IEnumerable<UserModel> afterAddGetContent = JsonConvert.DeserializeObject<IEnumerable<UserModel>>(afterAddGetContentString);
                    int afterAddGetContentCount = afterAddGetContent.Count();

                    // Assert
                    Assert.Equal(HttpStatusCode.OK, afterAddGetStatusCode);
                    Assert.Equal(initialGetContentCount + 1, afterAddGetContentCount);


                    // Delete
                    // Act
                    HttpResponseMessage deleteResponse = await _client.DeleteAsync($"{url}/{id}");
                    HttpStatusCode deleteStatusCode = deleteResponse.StatusCode;

                    // Assert
                    Assert.Equal(HttpStatusCode.NoContent, deleteStatusCode);


                    // Delete
                    // Act
                    deleteResponse = await _client.DeleteAsync($"{url}/{id}");
                    deleteStatusCode = deleteResponse.StatusCode;
                    string deleteGetContentString = await deleteResponse.Content.ReadAsStringAsync();

                    // Assert
                    error = JsonConvert.DeserializeObject<ErrorModel>(deleteGetContentString);
                    Assert.Equal(HttpStatusCode.NotFound, deleteStatusCode);
                    Assert.Equal(HttpStatusCode.NotFound, error.ERROR_CODE);
                    Assert.Equal($"{route} {notPresentText}", error.ERROR_MSG);

                    break;
                case HttpStatusCode.Conflict:
                    error = JsonConvert.DeserializeObject<ErrorModel>(postGetContentString);
                    Assert.Equal(HttpStatusCode.Conflict, error.ERROR_CODE);
                    Assert.Equal($"{route} {isAlreadyPresentText}", error.ERROR_MSG);
                    break;
                case HttpStatusCode.BadRequest:
                    error = JsonConvert.DeserializeObject<ErrorModel>(postGetContentString);
                    Assert.Equal(HttpStatusCode.BadRequest, error.ERROR_CODE);
                    Assert.Equal($"{nullField} {isRequiredText}", error.ERROR_MSG);
                    break;
                default:
                    Assert.True(false);
                    break;
            }
        }

        [Theory]
        [InlineData(1, "Test Project", "Details", "1-DEC-2021")]
        [InlineData(11, "Test Project", "Details", "1-DEC-2021")]
        [InlineData(11, null, "Details", "1-DEC-2021")]
        [InlineData(11, "Test Project", null, "1-DEC-2021")]
        [InlineData(11, null, null, "1-DEC-2021")]
        public async Task ProjectControllerTest(int id, string name, string details, string createdOn)
        {
            // Common
            DateTime createdOnDate = Convert.ToDateTime(createdOn);
            string route = "Project";
            string url = $"{apiUrl}/{route}";
            ErrorModel error;

            // Get All
            // Act
            HttpResponseMessage initialGetResponse = await _client.GetAsync(url);
            HttpStatusCode initialGetStatusCode = initialGetResponse.StatusCode;
            string initialGetContentString = await initialGetResponse.Content.ReadAsStringAsync();
            IEnumerable<ProjectModel> initialGetContent = JsonConvert.DeserializeObject<IEnumerable<ProjectModel>>(initialGetContentString);
            int initialGetContentCount = initialGetContent.Count();

            // Assert
            Assert.Equal(HttpStatusCode.OK, initialGetStatusCode);


            // Get By Id
            // Act
            HttpResponseMessage getByIdResponse = await _client.GetAsync($"{url}/{id}");
            HttpStatusCode getByIdStatusCode = getByIdResponse.StatusCode;
            string getByIdContentString = await getByIdResponse.Content.ReadAsStringAsync();

            // Assert
            switch (getByIdStatusCode)
            {
                case HttpStatusCode.OK:
                    int projectId = JsonConvert.DeserializeObject<ProjectModel>(getByIdContentString).ID_PROJECT;
                    Assert.Equal(id, projectId);
                    break;
                case HttpStatusCode.NotFound:
                    error = JsonConvert.DeserializeObject<ErrorModel>(getByIdContentString);
                    Assert.Equal(HttpStatusCode.NotFound, error.ERROR_CODE);
                    Assert.Equal($"{route} not present.", error.ERROR_MSG);
                    break;
                default:
                    Assert.True(false);
                    break;
            }


            // Post
            // Arrange
            ProjectModel projectModel = new ProjectModel() { ID_PROJECT = id, NAME = name, DETAILS = details, CREATED_ON = createdOnDate };
            HttpContent postContent = new StringContent(JsonConvert.SerializeObject(projectModel), Encoding.UTF8, "application/json");
            string nullField = null;
            if (string.IsNullOrEmpty(name))
                nullField = "Name";
            else if (string.IsNullOrEmpty(details))
                nullField = "Details";

            // Act
            HttpResponseMessage postReponse = await _client.PostAsync(url, postContent);
            HttpStatusCode postStatusCode = postReponse.StatusCode;
            string postGetContentString = await postReponse.Content.ReadAsStringAsync();

            // Assert
            switch (postStatusCode)
            {
                case HttpStatusCode.NoContent:
                    // Get All
                    // Act
                    HttpResponseMessage afterAddGetResponse = await _client.GetAsync(url);
                    HttpStatusCode afterAddGetStatusCode = afterAddGetResponse.StatusCode;
                    string afterAddGetContentString = await afterAddGetResponse.Content.ReadAsStringAsync();
                    IEnumerable<ProjectModel> afterAddGetContent = JsonConvert.DeserializeObject<IEnumerable<ProjectModel>>(afterAddGetContentString);
                    int afterAddGetContentCount = afterAddGetContent.Count();

                    // Assert
                    Assert.Equal(HttpStatusCode.OK, afterAddGetStatusCode);
                    Assert.Equal(initialGetContentCount + 1, afterAddGetContentCount);


                    // Delete
                    // Act
                    HttpResponseMessage deleteResponse = await _client.DeleteAsync($"{url}/{id}");
                    HttpStatusCode deleteStatusCode = deleteResponse.StatusCode;

                    // Assert
                    Assert.Equal(HttpStatusCode.NoContent, deleteStatusCode);


                    // Delete
                    // Act
                    deleteResponse = await _client.DeleteAsync($"{url}/{id}");
                    deleteStatusCode = deleteResponse.StatusCode;
                    string deleteGetContentString = await deleteResponse.Content.ReadAsStringAsync();

                    // Assert
                    error = JsonConvert.DeserializeObject<ErrorModel>(deleteGetContentString);
                    Assert.Equal(HttpStatusCode.NotFound, deleteStatusCode);
                    Assert.Equal(HttpStatusCode.NotFound, error.ERROR_CODE);
                    Assert.Equal($"{route} {notPresentText}", error.ERROR_MSG);

                    break;
                case HttpStatusCode.Conflict:
                    error = JsonConvert.DeserializeObject<ErrorModel>(postGetContentString);
                    Assert.Equal(HttpStatusCode.Conflict, error.ERROR_CODE);
                    Assert.Equal($"{route} {isAlreadyPresentText}", error.ERROR_MSG);
                    break;
                case HttpStatusCode.BadRequest:
                    error = JsonConvert.DeserializeObject<ErrorModel>(postGetContentString);
                    Assert.Equal(HttpStatusCode.BadRequest, error.ERROR_CODE);
                    Assert.Equal($"{nullField} {isRequiredText}", error.ERROR_MSG);
                    break;
                default:
                    Assert.True(false);
                    break;
            }
        }

        [Theory]
        [InlineData(1, "New", "Details", "1-DEC-2021")]
        [InlineData(11, "New", "Details", "1-DEC-2021")]
        [InlineData(11, "InProgress", "Details", "1-DEC-2021")]
        [InlineData(11, "QA", "Details", "1-DEC-2021")]
        [InlineData(11, "Completed", "Details", "1-DEC-2021")]
        [InlineData(11, null, "Details", "1-DEC-2021")]
        [InlineData(11, "New", null, "1-DEC-2021")]
        [InlineData(11, null, null, "1-DEC-2021")]
        [InlineData(1, "Neww", "Details", "1-DEC-2021")]
        [InlineData(11, "Neww", "Details", "1-DEC-2021")]
        public async Task TaskControllerTest(int id, string status, string details, string createdOn)
        {
            // Common
            DateTime createdOnDate = Convert.ToDateTime(createdOn);
            string route = "Task";
            string url = $"{apiUrl}/{route}";
            IEnumerable<string> validStatus = new List<string>() { "New", "InProgress", "QA", "Completed" };
            ErrorModel error;

            // Get All
            // Act
            HttpResponseMessage initialGetResponse = await _client.GetAsync(url);
            HttpStatusCode initialGetStatusCode = initialGetResponse.StatusCode;
            string initialGetContentString = await initialGetResponse.Content.ReadAsStringAsync();
            IEnumerable<ProjectModel> initialGetContent = JsonConvert.DeserializeObject<IEnumerable<ProjectModel>>(initialGetContentString);
            int initialGetContentCount = initialGetContent.Count();

            // Assert
            Assert.Equal(HttpStatusCode.OK, initialGetStatusCode);


            // Get By Id
            // Act
            HttpResponseMessage getByIdResponse = await _client.GetAsync($"{url}/{id}");
            HttpStatusCode getByIdStatusCode = getByIdResponse.StatusCode;
            string getByIdContentString = await getByIdResponse.Content.ReadAsStringAsync();

            // Assert
            switch (getByIdStatusCode)
            {
                case HttpStatusCode.OK:
                    int taskId = JsonConvert.DeserializeObject<TaskModel>(getByIdContentString).ID_TASK;
                    Assert.Equal(id, taskId);
                    break;
                case HttpStatusCode.NotFound:
                    error = JsonConvert.DeserializeObject<ErrorModel>(getByIdContentString);
                    Assert.Equal(HttpStatusCode.NotFound, error.ERROR_CODE);
                    Assert.Equal($"{route} not present.", error.ERROR_MSG);
                    break;
                default:
                    Assert.True(false);
                    break;
            }

            // Get Project and User Model
            HttpResponseMessage projectReponse = await _client.GetAsync($"{apiUrl}/Project/1");
            HttpStatusCode projectStatusCode = projectReponse.StatusCode;

            HttpResponseMessage userReponse = await _client.GetAsync($"{apiUrl}/User/1");
            HttpStatusCode userStatusCode = userReponse.StatusCode;


            if (projectStatusCode.Equals(HttpStatusCode.OK) && userStatusCode.Equals(HttpStatusCode.OK))
            {
                string projectContentString = await projectReponse.Content.ReadAsStringAsync();
                ProjectModel projectModel = JsonConvert.DeserializeObject<ProjectModel>(projectContentString);

                string userContentString = await userReponse.Content.ReadAsStringAsync();
                UserModel userModel = JsonConvert.DeserializeObject<UserModel>(userContentString);


                // Post
                // Arrange
                TaskModel taskModel = new TaskModel()
                {
                    ID_TASK = id,
                    DETAILS = details,
                    CREATED_ON = createdOnDate,
                    STATUS = status,
                    PROJECT = projectModel,
                    USER = userModel
                };
                HttpContent postContent = new StringContent(JsonConvert.SerializeObject(taskModel), Encoding.UTF8, "application/json");
                string nullField = null;
                if (string.IsNullOrEmpty(details))
                    nullField = "Details";
                else if (string.IsNullOrEmpty(status))
                    nullField = "Status";

                // Act
                HttpResponseMessage postReponse = await _client.PostAsync(url, postContent);
                HttpStatusCode postStatusCode = postReponse.StatusCode;
                string postGetContentString = await postReponse.Content.ReadAsStringAsync();

                // Assert
                switch (postStatusCode)
                {
                    case HttpStatusCode.NoContent:
                        // Get All
                        // Act
                        HttpResponseMessage afterAddGetResponse = await _client.GetAsync(url);
                        HttpStatusCode afterAddGetStatusCode = afterAddGetResponse.StatusCode;
                        string afterAddGetContentString = await afterAddGetResponse.Content.ReadAsStringAsync();
                        IEnumerable<UserModel> afterAddGetContent = JsonConvert.DeserializeObject<IEnumerable<UserModel>>(afterAddGetContentString);
                        int afterAddGetContentCount = afterAddGetContent.Count();

                        // Assert
                        Assert.Equal(HttpStatusCode.OK, afterAddGetStatusCode);
                        Assert.Equal(initialGetContentCount + 1, afterAddGetContentCount);


                        // Delete
                        // Act
                        HttpResponseMessage deleteResponse = await _client.DeleteAsync($"{url}/{id}");
                        HttpStatusCode deleteStatusCode = deleteResponse.StatusCode;

                        // Assert
                        Assert.Equal(HttpStatusCode.NoContent, deleteStatusCode);


                        // Delete
                        // Act
                        deleteResponse = await _client.DeleteAsync($"{url}/{id}");
                        deleteStatusCode = deleteResponse.StatusCode;
                        string deleteGetContentString = await deleteResponse.Content.ReadAsStringAsync();

                        // Assert
                        error = JsonConvert.DeserializeObject<ErrorModel>(deleteGetContentString);
                        Assert.Equal(HttpStatusCode.NotFound, deleteStatusCode);
                        Assert.Equal(HttpStatusCode.NotFound, error.ERROR_CODE);
                        Assert.Equal($"{route} {notPresentText}", error.ERROR_MSG);

                        break;
                    case HttpStatusCode.Conflict:
                        error = JsonConvert.DeserializeObject<ErrorModel>(postGetContentString);
                        Assert.Equal(HttpStatusCode.Conflict, error.ERROR_CODE);
                        Assert.Equal($"{route} {isAlreadyPresentText}", error.ERROR_MSG);
                        break;
                    case HttpStatusCode.BadRequest:
                        error = JsonConvert.DeserializeObject<ErrorModel>(postGetContentString);
                        IEnumerable<string> expectedErrors = new List<string>() { $"Invalid Status Value - {status}", $"{nullField} {isRequiredText}" };
                        Assert.Equal(HttpStatusCode.BadRequest, error.ERROR_CODE);
                        Assert.Contains(error.ERROR_MSG, expectedErrors);
                        break;
                    default:
                        Assert.True(false);
                        break;
                }
            }
        }
    }
}
