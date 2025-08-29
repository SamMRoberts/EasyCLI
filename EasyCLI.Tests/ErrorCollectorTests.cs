using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using EasyCLI.Console;
using Xunit;

namespace EasyCLI.Tests
{
    /// <summary>
    /// Tests for ErrorCollector functionality including error grouping and aggregation.
    /// </summary>
    public class ErrorCollectorTests
    {
        private ErrorCollector CreateErrorCollector()
        {
            var writer = new ConsoleWriter(enableColors: false, output: new StringWriter());
            return new ErrorCollector(writer);
        }

        [Fact]
        public void Constructor_WithNullWriter_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ErrorCollector(null!));
        }

        [Fact]
        public void Constructor_WithValidWriter_InitializesCorrectly()
        {
            // Arrange
            var writer = new ConsoleWriter();

            // Act
            var collector = new ErrorCollector(writer);

            // Assert
            Assert.Equal(0, collector.TotalCount);
            Assert.False(collector.HasErrors);
            Assert.Empty(collector.Errors);
        }

        [Fact]
        public void AddError_WithCategoryAndMessage_AddsErrorCorrectly()
        {
            // Arrange
            var collector = CreateErrorCollector();

            // Act
            collector.AddError(BatchErrorCategory.FileSystem, "File not found", "test.txt");

            // Assert
            Assert.Equal(1, collector.TotalCount);
            Assert.True(collector.HasErrors);
            var error = collector.Errors.Single();
            Assert.Equal(BatchErrorCategory.FileSystem, error.Category);
            Assert.Equal("File not found", error.Message);
            Assert.Equal("test.txt", error.Source);
        }

        [Fact]
        public void AddError_WithOnlyMessage_AddsAsGeneralError()
        {
            // Arrange
            var collector = CreateErrorCollector();

            // Act
            collector.AddError("General error message");

            // Assert
            Assert.Equal(1, collector.TotalCount);
            var error = collector.Errors.Single();
            Assert.Equal(BatchErrorCategory.General, error.Category);
            Assert.Equal("General error message", error.Message);
        }

        [Fact]
        public void AddError_WithException_InfersCategoryCorrectly()
        {
            // Arrange
            var collector = CreateErrorCollector();

            // Act
            collector.AddError(new FileNotFoundException("File not found"), "operation1");
            collector.AddError(new ArgumentNullException("param"), "operation2");
            collector.AddError(new SecurityException("Access denied"), "operation3");
            collector.AddError(new HttpRequestException("Network error"), "operation4");

            // Assert
            Assert.Equal(4, collector.TotalCount);
            var errors = collector.Errors.ToList();

            Assert.Equal(BatchErrorCategory.FileSystem, errors[0].Category);
            Assert.Equal(BatchErrorCategory.Validation, errors[1].Category);
            Assert.Equal(BatchErrorCategory.Security, errors[2].Category);
            Assert.Equal(BatchErrorCategory.Network, errors[3].Category);
        }

        [Fact]
        public void AddError_WithNullMessage_ThrowsArgumentNullException()
        {
            // Arrange
            var collector = CreateErrorCollector();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => collector.AddError(BatchErrorCategory.General, null!));
        }

        [Fact]
        public void AddError_WithNullException_ThrowsArgumentNullException()
        {
            // Arrange
            var collector = CreateErrorCollector();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => collector.AddError((Exception)null!));
        }

        [Fact]
        public void GetSummaries_WithMultipleErrors_GroupsCorrectly()
        {
            // Arrange
            var collector = CreateErrorCollector();
            collector.AddError(BatchErrorCategory.FileSystem, "Error 1");
            collector.AddError(BatchErrorCategory.FileSystem, "Error 2");
            collector.AddError(BatchErrorCategory.Network, "Error 3");
            collector.AddError(BatchErrorCategory.Validation, "Error 4");
            collector.AddError(BatchErrorCategory.Validation, "Error 5");
            collector.AddError(BatchErrorCategory.Validation, "Error 6");

            // Act
            var summaries = collector.GetSummaries().ToList();

            // Assert
            Assert.Equal(3, summaries.Count);
            
            var fileSystemSummary = summaries.First(s => s.Category == BatchErrorCategory.FileSystem);
            Assert.Equal(2, fileSystemSummary.Count);
            Assert.Equal("File System", fileSystemSummary.CategoryDisplayName);

            var networkSummary = summaries.First(s => s.Category == BatchErrorCategory.Network);
            Assert.Equal(1, networkSummary.Count);
            Assert.Equal("Network", networkSummary.CategoryDisplayName);

            var validationSummary = summaries.First(s => s.Category == BatchErrorCategory.Validation);
            Assert.Equal(3, validationSummary.Count);
            Assert.Equal("Validation", validationSummary.CategoryDisplayName);
        }

        [Fact]
        public void GetSummaries_WithCategoryFilter_ReturnsFilteredResults()
        {
            // Arrange
            var collector = CreateErrorCollector();
            collector.AddError(BatchErrorCategory.FileSystem, "Error 1");
            collector.AddError(BatchErrorCategory.Network, "Error 2");
            collector.AddError(BatchErrorCategory.Validation, "Error 3");

            // Act
            var summaries = collector.GetSummaries(BatchErrorCategory.FileSystem, BatchErrorCategory.Network).ToList();

            // Assert
            Assert.Equal(2, summaries.Count);
            Assert.Contains(summaries, s => s.Category == BatchErrorCategory.FileSystem);
            Assert.Contains(summaries, s => s.Category == BatchErrorCategory.Network);
            Assert.DoesNotContain(summaries, s => s.Category == BatchErrorCategory.Validation);
        }

        [Fact]
        public void Clear_RemovesAllErrors()
        {
            // Arrange
            var collector = CreateErrorCollector();
            collector.AddError("Error 1");
            collector.AddError("Error 2");

            // Act
            collector.Clear();

            // Assert
            Assert.Equal(0, collector.TotalCount);
            Assert.False(collector.HasErrors);
            Assert.Empty(collector.Errors);
        }

        [Fact]
        public void PrintSummary_WithNoErrors_ShowsSuccessMessage()
        {
            // Arrange
            var output = new StringWriter();
            var writer = new ConsoleWriter(enableColors: false, output: output);
            var collector = new ErrorCollector(writer);

            // Act
            collector.PrintSummary();

            // Assert
            var result = output.ToString();
            Assert.Contains("✓ No errors to report", result);
        }

        [Fact]
        public void PrintSummary_WithErrors_ShowsSummaryTable()
        {
            // Arrange
            var output = new StringWriter();
            var writer = new ConsoleWriter(enableColors: false, output: output);
            var collector = new ErrorCollector(writer);
            
            collector.AddError(BatchErrorCategory.FileSystem, "File error 1");
            collector.AddError(BatchErrorCategory.FileSystem, "File error 2");
            collector.AddError(BatchErrorCategory.Network, "Network error");

            // Act
            collector.PrintSummary(showDetails: false);

            // Assert
            var result = output.ToString();
            Assert.Contains("Error Summary (3 total)", result);
            Assert.Contains("File System", result);
            Assert.Contains("Network", result);
            Assert.Contains("66.7%", result); // 2/3 for FileSystem
            Assert.Contains("33.3%", result); // 1/3 for Network
            Assert.Contains("💡 Use --verbose or --details", result);
        }

        [Fact]
        public void PrintSummary_WithDetailsEnabled_ShowsDetailedErrors()
        {
            // Arrange
            var output = new StringWriter();
            var writer = new ConsoleWriter(enableColors: false, output: output);
            var collector = new ErrorCollector(writer);
            
            collector.AddError(BatchErrorCategory.FileSystem, "File not found", "test.txt", "IOException");

            // Act
            collector.PrintSummary(showDetails: true);

            // Assert
            var result = output.ToString();
            Assert.Contains("File System (1)", result);
            Assert.Contains("File not found", result);
            Assert.Contains("Source: test.txt", result);
            Assert.Contains("Details: IOException", result);
        }

        [Fact]
        public void PrintCategoryDetails_WithExistingCategory_ShowsDetails()
        {
            // Arrange
            var output = new StringWriter();
            var writer = new ConsoleWriter(enableColors: false, output: output);
            var collector = new ErrorCollector(writer);
            
            collector.AddError(BatchErrorCategory.FileSystem, "File error 1", "file1.txt");
            collector.AddError(BatchErrorCategory.FileSystem, "File error 2", "file2.txt");

            // Act
            collector.PrintCategoryDetails(BatchErrorCategory.FileSystem);

            // Assert
            var result = output.ToString();
            Assert.Contains("File System Errors (2)", result);
            Assert.Contains("01. File error 1", result);
            Assert.Contains("02. File error 2", result);
            Assert.Contains("Source: file1.txt", result);
            Assert.Contains("Source: file2.txt", result);
        }

        [Fact]
        public void PrintCategoryDetails_WithEmptyCategory_ShowsNoErrorsMessage()
        {
            // Arrange
            var output = new StringWriter();
            var writer = new ConsoleWriter(enableColors: false, output: output);
            var collector = new ErrorCollector(writer);

            // Act
            collector.PrintCategoryDetails(BatchErrorCategory.FileSystem);

            // Assert
            var result = output.ToString();
            Assert.Contains("No errors in category: FileSystem", result);
        }

        [Fact]
        public void CollectedError_ToString_FormatsCorrectly()
        {
            // Arrange
            var error1 = new CollectedError(BatchErrorCategory.General, "Simple message");
            var error2 = new CollectedError(BatchErrorCategory.FileSystem, "File error", "test.txt");
            var error3 = new CollectedError(BatchErrorCategory.Network, "Network error", "service", "TimeoutException");

            // Act & Assert
            Assert.Equal("Simple message", error1.ToString());
            Assert.Equal("File error | Source: test.txt", error2.ToString());
            Assert.Equal("Network error | Source: service | Details: TimeoutException", error3.ToString());
        }

        [Fact]
        public void ErrorSummary_CategoryDisplayName_FormatsCorrectly()
        {
            // Arrange
            var errors = new List<CollectedError>
            {
                new(BatchErrorCategory.FileSystem, "Error"),
            };

            // Act
            var fileSystemSummary = new ErrorSummary(BatchErrorCategory.FileSystem, errors);
            var externalServiceSummary = new ErrorSummary(BatchErrorCategory.ExternalService, errors);
            var invalidArgumentSummary = new ErrorSummary(BatchErrorCategory.InvalidArgument, errors);
            var generalSummary = new ErrorSummary(BatchErrorCategory.General, errors);

            // Assert
            Assert.Equal("File System", fileSystemSummary.CategoryDisplayName);
            Assert.Equal("External Service", externalServiceSummary.CategoryDisplayName);
            Assert.Equal("Invalid Argument", invalidArgumentSummary.CategoryDisplayName);
            Assert.Equal("General", generalSummary.CategoryDisplayName);
        }

        [Fact]
        public void AddError_MultipleSimilarErrors_AccumulatesCorrectly()
        {
            // Arrange
            var collector = CreateErrorCollector();

            // Act
            for (int i = 0; i < 5; i++)
            {
                collector.AddError(BatchErrorCategory.FileSystem, $"File {i} not found", $"file{i}.txt");
            }

            for (int i = 0; i < 3; i++)
            {
                collector.AddError(BatchErrorCategory.Network, $"Connection {i} failed", $"server{i}");
            }

            // Assert
            Assert.Equal(8, collector.TotalCount);
            
            var summaries = collector.GetSummaries().ToList();
            Assert.Equal(2, summaries.Count);
            
            var fileSystemSummary = summaries.First(s => s.Category == BatchErrorCategory.FileSystem);
            Assert.Equal(5, fileSystemSummary.Count);
            
            var networkSummary = summaries.First(s => s.Category == BatchErrorCategory.Network);
            Assert.Equal(3, networkSummary.Count);
        }

        [Fact]
        public void ErrorCollector_WithDifferentExceptionTypes_CategorizesCorrectly()
        {
            // Arrange
            var collector = CreateErrorCollector();

            // Act
            collector.AddError(new DirectoryNotFoundException("Directory missing"));
            collector.AddError(new UnauthorizedAccessException("Access denied"));
            collector.AddError(new ArgumentOutOfRangeException("Index out of range"));
            collector.AddError(new InvalidOperationException("Invalid state")); // Should be General

            // Assert
            var summaries = collector.GetSummaries().ToList();
            Assert.Equal(3, summaries.Count); // FileSystem, Validation, General

            var fileSystemErrors = summaries.First(s => s.Category == BatchErrorCategory.FileSystem).Errors;
            Assert.Equal(2, fileSystemErrors.Count); // DirectoryNotFoundException, UnauthorizedAccessException

            var validationErrors = summaries.First(s => s.Category == BatchErrorCategory.Validation).Errors;
            Assert.Single(validationErrors); // ArgumentOutOfRangeException

            var generalErrors = summaries.First(s => s.Category == BatchErrorCategory.General).Errors;
            Assert.Single(generalErrors); // InvalidOperationException
        }
    }
}