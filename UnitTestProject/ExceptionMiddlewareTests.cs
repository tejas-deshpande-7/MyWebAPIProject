using Xunit;
using Moq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TestAPIProject.Middlewares;
using System.IO;
using System.Text.Json;
using System;
using System.Threading.Tasks;

namespace UnitTestProject;

public class ExceptionMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_Returns500AndGenericMessage_InProduction()
    {
        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(e => e.EnvironmentName).Returns("Production");
        var mockLogger = new Mock<ILogger<ExceptionMiddleware>>();

        RequestDelegate next = (HttpContext ctx) => throw new Exception("boom");
        var middleware = new ExceptionMiddleware(next, mockLogger.Object, mockEnv.Object);

        var context = new DefaultHttpContext();
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var payload = new StreamReader(context.Response.Body).ReadToEnd();

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        using var doc = JsonDocument.Parse(payload);
        Assert.Equal("An unexpected error occurred.", doc.RootElement.GetProperty("error").GetString());
        Assert.False(doc.RootElement.TryGetProperty("detail", out _));
    }

    [Fact]
    public async Task InvokeAsync_InDevelopment_IncludesDetail()
    {
        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(e => e.EnvironmentName).Returns("Development");
        var mockLogger = new Mock<ILogger<ExceptionMiddleware>>();

        RequestDelegate next = (HttpContext ctx) => throw new InvalidOperationException("oops");
        var middleware = new ExceptionMiddleware(next, mockLogger.Object, mockEnv.Object);

        var context = new DefaultHttpContext();
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var payload = new StreamReader(context.Response.Body).ReadToEnd();

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        using var doc = JsonDocument.Parse(payload);
        Assert.True(doc.RootElement.TryGetProperty("detail", out var detail));
        Assert.Contains("InvalidOperationException", detail.GetString());
    }
}
