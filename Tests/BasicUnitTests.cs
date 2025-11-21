using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PROG6212_Part2.Controllers;
using PROG6212_Part2.Models;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using Xunit;

namespace PROG6212_Part2.Tests
{
    public class CollegeClaimsSystemTests : IDisposable
    {
        private readonly Mock<HttpContext> _mockHttpContext;
        private readonly Mock<ISession> _mockSession;

        public CollegeClaimsSystemTests()
        {
            _mockHttpContext = new Mock<HttpContext>();
            _mockSession = new Mock<ISession>();
            _mockHttpContext.Setup(c => c.Session).Returns(_mockSession.Object);

            // Clear test data before each test
            ClearTestData();
        }

        public void Dispose()
        {
            // Clear test data after each test
            ClearTestData();
        }

        private void ClearTestData()
        {
            // Clear only test-related claims, preserve demo data
            var testClaims = ClaimController._claims
                .Where(c => c.LecturerName?.Contains("Test") == true || c.ClaimID > 1000)
                .ToList();

            foreach (var claim in testClaims)
            {
                ClaimController._claims.Remove(claim);
            }
        }

        private void SetupSessionRole(string role, string email = "test@example.com")
        {
            _mockSession.Setup(s => s.GetString("Role")).Returns(role);
            _mockSession.Setup(s => s.GetString("Email")).Returns(email);
        }

        // ===== ACCOUNT CONTROLLER TESTS =====
        [Fact]
        public void HashPassword_ShouldReturnConsistentHash()
        {
            // Arrange
            var password = "test123";

            // Act
            var hash1 = AccountController.HashPassword(password);
            var hash2 = AccountController.HashPassword(password);

            // Assert
            Assert.Equal(hash1, hash2);
            Assert.NotNull(hash1);
            Assert.NotEqual(password, hash1);
        }

        [Fact]
        public void HashPassword_DifferentPasswords_ShouldReturnDifferentHashes()
        {
            // Arrange
            var password1 = "test123";
            var password2 = "test124";

            // Act
            var hash1 = AccountController.HashPassword(password1);
            var hash2 = AccountController.HashPassword(password2);

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void Login_WithValidLecturerCredentials_ShouldRedirectToSubmitClaim()
        {
            // Arrange
            var controller = new AccountController();
            var model = new LoginViewModel
            {
                Email = "zaara.salie@prog.ac.za",
                Password = "lecturer123"
            };

            SetupSessionRole("Lecturer");
            controller.ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object };

            // Act
            var result = controller.Login(model) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SubmitClaim", result.ActionName);
            Assert.Equal("Lecturer", result.ControllerName);
        }

        [Fact]
        public void Login_WithInvalidCredentials_ShouldReturnViewWithError()
        {
            // Arrange
            var controller = new AccountController();
            var model = new LoginViewModel
            {
                Email = "invalid@email.com",
                Password = "wrongpassword"
            };

            // Act
            var result = controller.Login(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public void Login_Get_ShouldReturnViewWithModel()
        {
            // Arrange
            var controller = new AccountController();

            // Act
            var result = controller.Login() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            Assert.IsType<LoginViewModel>(result.Model);
        }

        // ===== CLAIM CONTROLLER TESTS =====
        [Fact]
        public void Verify_WithValidClaimId_ShouldUpdateStatusToVerified()
        {
            // Arrange
            var controller = new ClaimController();
            var claim = new Claim
            {
                ClaimID = 9999,
                Status = "Submitted",
                LecturerName = "Test Lecturer",
                Department = "Test Department",
                Month = "January",
                CourseCode = "TEST101",
                CourseTitle = "Test Course",
                HoursWorked = 10,
                Rate = 100
            };
            ClaimController._claims.Add(claim);

            SetupSessionRole("Coordinator");
            controller.ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object };

            // Act
            var result = controller.Verify(9999) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Verified", claim.Status);
            Assert.Equal("CoordinatorView", result.ActionName);
        }

        [Fact]
        public void Approve_WithValidClaimId_ShouldUpdateStatusToApproved()
        {
            // Arrange
            var controller = new ClaimController();
            var claim = new Claim
            {
                ClaimID = 9998,
                Status = "Verified",
                LecturerName = "Test Lecturer",
                Department = "Test Department",
                Month = "January",
                CourseCode = "TEST101",
                CourseTitle = "Test Course",
                HoursWorked = 10,
                Rate = 100
            };
            ClaimController._claims.Add(claim);

            SetupSessionRole("Manager");
            controller.ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object };

            // Act
            var result = controller.Approve(9998) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Approved", claim.Status);
            Assert.Equal("ManagerView", result.ActionName);
        }

        [Fact]
        public void SaveEncryptedFile_ShouldCreateEncryptedFile()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var content = "test content for encryption";
            var fileName = "test_document.pdf";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));

            mockFile.Setup(f => f.CopyTo(It.IsAny<Stream>())).Callback<Stream>(s => ms.CopyTo(s));
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(ms.Length);

            var testPath = Path.Combine(Path.GetTempPath(), $"test_encrypted_{Guid.NewGuid()}.pdf");

            try
            {
                // Act
                ClaimController.SaveEncryptedFile(mockFile.Object, testPath);

                // Assert
                Assert.True(File.Exists(testPath));

                // Verify file was encrypted (content should not match original)
                var encryptedContent = File.ReadAllText(testPath);
                Assert.NotEqual(content, encryptedContent);
            }
            finally
            {
                // Cleanup
                if (File.Exists(testPath))
                    File.Delete(testPath);
            }
        }

        [Fact]
        public void Download_WithInvalidClaimId_ShouldReturnNotFound()
        {
            // Arrange
            var controller = new ClaimController();

            // Act
            var result = controller.Download(99999); // Non-existent ID

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // ===== LECTURER CONTROLLER TESTS =====
        [Fact]
        public void SubmitClaim_WithValidDataAndFile_ShouldAddClaimToList()
        {
            // Arrange
            var controller = new LecturerController();
            var claim = new Claim
            {
                Month = "January",
                CourseCode = "PROG6212",
                CourseTitle = "Programming",
                HoursWorked = 20,
                Rate = 300,
                Notes = "Test claim"
            };

            var mockFile = CreateMockFile("test.pdf", "test content");

            SetupSessionRole("Lecturer", "zaara.salie@prog.ac.za");
            controller.ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object };

            var initialCount = ClaimController._claims.Count;

            // Act
            var result = controller.SubmitClaim(claim, mockFile.Object) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(initialCount + 1, ClaimController._claims.Count);
            Assert.NotNull(result.ViewBag.Message);
            Assert.Contains("successfully", result.ViewBag.Message.ToString().ToLower());
        }

        [Fact]
        public void SubmitClaim_WithInvalidFileType_ShouldReturnError()
        {
            // Arrange
            var controller = new LecturerController();
            var claim = new Claim
            {
                Month = "January",
                CourseCode = "PROG6212",
                CourseTitle = "Programming",
                HoursWorked = 20,
                Rate = 300
            };

            var mockFile = CreateMockFile("test.txt", "invalid file type");

            SetupSessionRole("Lecturer", "zaara.salie@prog.ac.za");
            controller.ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object };

            // Act
            var result = controller.SubmitClaim(claim, mockFile.Object) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.ViewBag.Error);
            Assert.Contains("allowed", result.ViewBag.Error.ToString().ToLower());
        }

        [Fact]
        public void SubmitClaim_WithExcessiveHours_ShouldReturnError()
        {
            // Arrange
            var controller = new LecturerController();
            var claim = new Claim
            {
                Month = "January",
                CourseCode = "PROG6212",
                CourseTitle = "Programming",
                HoursWorked = 200, // Exceeds 160 limit
                Rate = 300
            };

            var mockFile = CreateMockFile("test.pdf", "test content");

            SetupSessionRole("Lecturer", "zaara.salie@prog.ac.za");
            controller.ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object };

            // Act
            var result = controller.SubmitClaim(claim, mockFile.Object) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.ViewBag.Error);
            Assert.Contains("exceed", result.ViewBag.Error.ToString().ToLower());
        }

        [Fact]
        public void ViewClaims_AsLecturer_ShouldReturnOnlyTheirClaims()
        {
            // Arrange
            var controller = new LecturerController();

            // Add test claims
            var lecturerClaim = new Claim
            {
                ClaimID = 9997,
                LecturerName = "Zaara Salie",
                Status = "Submitted"
            };
            var otherClaim = new Claim
            {
                ClaimID = 9996,
                LecturerName = "Other Lecturer",
                Status = "Submitted"
            };

            ClaimController._claims.Add(lecturerClaim);
            ClaimController._claims.Add(otherClaim);

            SetupSessionRole("Lecturer", "zaara.salie@prog.ac.za");
            controller.ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object };

            // Act
            var result = controller.ViewClaims() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as List<Claim>;
            Assert.NotNull(model);
            Assert.All(model, c => Assert.Equal("Zaara Salie", c.LecturerName));
        }

        // ===== MODEL VALIDATION TESTS =====
        [Fact]
        public void Claim_WithValidData_ShouldPassValidation()
        {
            // Arrange
            var claim = new Claim
            {
                LecturerName = "Test Lecturer",
                Department = "Computer Science",
                Month = "January",
                CourseCode = "PROG6212",
                CourseTitle = "Programming",
                HoursWorked = 20,
                Rate = 300
            };

            var context = new ValidationContext(claim);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(claim, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Claim_WithMissingRequiredFields_ShouldFailValidation()
        {
            // Arrange
            var claim = new Claim(); // Missing required fields

            var context = new ValidationContext(claim);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(claim, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void Claim_TotalAmount_ShouldCalculateCorrectly()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 10,
                Rate = 150
            };

            // Act
            var totalAmount = claim.TotalAmount;

            // Assert
            Assert.Equal(1500, totalAmount);
        }

        [Fact]
        public void LoginViewModel_WithValidData_ShouldPassValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void LoginViewModel_WithInvalidEmail_ShouldFailValidation()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "invalid-email",
                Password = "password123"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.NotEmpty(results);
        }

        // ===== HELPER METHODS =====
        private Mock<IFormFile> CreateMockFile(string fileName, string content)
        {
            var mockFile = new Mock<IFormFile>();
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));

            mockFile.Setup(f => f.CopyTo(It.IsAny<Stream>())).Callback<Stream>(s => ms.CopyTo(s));
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(ms.Length);
            mockFile.Setup(f => f.OpenReadStream()).Returns(ms);

            return mockFile;
        }
    }
}