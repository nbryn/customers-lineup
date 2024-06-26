﻿using System.Net;
using System.Net.Http.Headers;
using System.Text;
using CLup.API.Auth.Contracts;
using CLup.API.Businesses.Contracts;
using CLup.API.Businesses.Contracts.CreateBusiness;
using CLup.API.Businesses.Contracts.GetBusinessAggregate;
using CLup.API.Users.Contracts.GetUser;
using CLup.Application.Businesses;
using CLup.Application.Shared;
using CLup.Application.Users;
using CLup.Domain.Businesses.Enums;
using CLup.Domain.Shared.ValueObjects;
using CLup.Domain.Users.Enums;
using CLup.Domain.Users.ValueObjects;
using CLup.Infrastructure.Persistence.Seed.Builders;
using Newtonsoft.Json;

namespace tests.CLup.IntegrationTests;

public abstract class IntegrationTestsBase : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _httpClient;
    protected static string BaseRoute = "api";
    protected string BookingRoute { get; } = $"{BaseRoute}/booking";
    protected string BusinessRoute { get; } = $"{BaseRoute}/business";
    protected string EmployeeRoute { get; } = $"{BaseRoute}/employee";
    protected string MessageRoute { get; } = $"{BaseRoute}/message";
    protected string TimeSlotRoute { get; } = $"{BaseRoute}/timeslot";
    protected string QueryRoute { get; } = $"{BaseRoute}/query";
    protected string UserRoute { get; } = $"{BaseRoute}/user";

    protected IntegrationTestsBase(IntegrationTestWebAppFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    protected async Task<(Guid UserId, BusinessAggregateDto Business)> CreateUserWithBusiness(
        int capacity = 50,
        TimeOnly? opens = null,
        TimeOnly? closes = null,
        int timeSlotLengthInMinutes = 30)
    {
        var userId = await CreateUserAndSetJwtToken();
        var business = new BusinessBuilder()
            .WithOwner(UserId.Create(userId))
            .WithBusinessData("Super Brugsen", capacity, timeSlotLengthInMinutes)
            .WithBusinessHours(opens ?? new TimeOnly(10, 0), closes ?? new TimeOnly(22, 0))
            .WithAddress("Ryttergårdsvej 10", 3520, "Farum", new Coords(55.8137419, 12.3935222))
            .WithType(BusinessType.Supermarket)
            .Build();

        var request = new CreateBusinessRequest
        {
            Name = business.BusinessData.Name,
            Capacity = business.BusinessData.Capacity,
            TimeSlotLengthInMinutes = business.BusinessData.TimeSlotLengthInMinutes,
            Address = business.Address,
            BusinessHours = business.BusinessHours,
            Type = business.Type
        };

        await PostAsyncAndEnsureSuccess(BusinessRoute, request);
        var businessAggregate = (await GetBusinessesForCurrentUser()).First();

        return (userId, businessAggregate);
    }

    protected async Task<Guid> CreateUserAndSetJwtToken(string? password = null)
    {
        var userEmail = $"{Guid.NewGuid()}@test.com";
        var newUser = new UserBuilder()
            .WithUserData("Peter", userEmail, password ?? "1234")
            .WithAddress("Farum Hovedgade 15", 3520, "Farum", new Coords(55.8122540, 12.3706760))
            .WithRole(Role.User)
            .Build();

        var registerRequest = new RegisterRequest()
        {
            Email = newUser.UserData.Email,
            Password = newUser.UserData.Password,
            Name = newUser.UserData.Name,
            Address = newUser.Address
        };

        var tokenResponse = await PostAsyncAndEnsureSuccess<RegisterRequest, TokenResponse>(
            $"{BaseRoute}/register",
            registerRequest);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);

        var user = await GetUser();
        return user.Id;
    }

    protected async Task<UserDto> GetUser()
    {
        if (_httpClient.DefaultRequestHeaders.Authorization == null)
        {
            throw new InvalidOperationException("Not authenticated");
        }

        var response = await GetAsyncAndEnsureSuccess<GetUserResponse>($"{QueryRoute}/user");
        response.Should().NotBeNull();

        return response.User;
    }

    protected async Task<BusinessAggregateDto> GetBusinessAggregate(Guid businessId)
    {
        if (_httpClient.DefaultRequestHeaders.Authorization == null)
        {
            throw new InvalidOperationException("Not authenticated");
        }

        var response = await GetAsyncAndEnsureSuccess<GetBusinessAggregateResponse>($"{QueryRoute}/business/aggregate/{businessId}");
        response.Should().NotBeNull();

        return response.Business;
    }

    protected async Task Login(string email, string password)
    {
        var request = new LoginRequest(email, password);
        var response = await PostAsyncAndEnsureSuccess<LoginRequest, TokenResponse>($"{BaseRoute}/login", request);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.Token);
    }

    protected async Task<IList<BusinessAggregateDto>> GetBusinessesForCurrentUser()
    {
        var response = await GetAsyncAndEnsureSuccess<GetBusinessesByOwnerResponse>($"{QueryRoute}/user/businesses");
        response.Should().NotBeNull();

        return response.Businesses;
    }

    protected async Task<TResult?> GetAsyncAndEnsureSuccess<TResult>(string url) =>
        await GetAsyncAndEnsureStatus<TResult>(url, HttpStatusCode.OK);

    protected async Task DeleteAsyncAndEnsureSuccess(string url) =>
        await DeleteAsyncAndEnsureStatus(url, HttpStatusCode.OK);

    protected async Task<ProblemDetails?> DeleteAsyncAndEnsureNotFound(string url) =>
        await DeleteAsyncAndEnsureStatus<ProblemDetails>(url, HttpStatusCode.NotFound);

    protected async Task<ProblemDetails?> DeleteAsyncAndEnsureBadRequest(string url) =>
        await DeleteAsyncAndEnsureStatus<ProblemDetails>(url, HttpStatusCode.BadRequest);

    protected async Task PutAsyncAndEnsureSuccess<TRequest>(string url, TRequest request) =>
        await PutAsyncAndEnsureStatus(url, request, HttpStatusCode.OK);

    protected async Task<ProblemDetails?> PutAsyncAndEnsureBadRequest<TRequest>(string url, TRequest request) =>
        await PutAsyncAndEnsureStatus<TRequest, ProblemDetails>(url, request, HttpStatusCode.BadRequest);

    protected async Task PatchAsyncAndEnsureSuccess<TRequest>(string url, TRequest request) =>
        await PatchAsyncAndEnsureStatus(url, request, HttpStatusCode.OK);

    protected async Task<ProblemDetails?> PatchAsyncAndEnsureNotFound<TRequest>(string url, TRequest request) =>
        await PatchAsyncAndEnsureStatus<TRequest, ProblemDetails>(url, request, HttpStatusCode.NotFound);

    protected async Task<ProblemDetails?> PatchAsyncAndEnsureBadRequest<TRequest>(string url, TRequest request) =>
        await PatchAsyncAndEnsureStatus<TRequest, ProblemDetails>(url, request, HttpStatusCode.BadRequest);

    protected async Task PostAsyncAndEnsureSuccess<TRequest>(string url, TRequest request) =>
        await PostAsyncAndEnsureStatus(url, request, HttpStatusCode.OK);

    protected async Task<TResult?> PostAsyncAndEnsureSuccess<TRequest, TResult>(string url, TRequest request) =>
        await PostAsyncAndEnsureStatus<TRequest, TResult>(url, request, HttpStatusCode.OK);

    protected async Task<ProblemDetails?> PostAsyncAndEnsureNotFound<TRequest>(string url, TRequest request) =>
        await PostAsyncAndEnsureStatus<TRequest, ProblemDetails>(url, request, HttpStatusCode.NotFound);

    protected async Task<ProblemDetails?> PostAsyncAndEnsureBadRequest<TRequest>(string url, TRequest request) =>
        await PostAsyncAndEnsureStatus<TRequest, ProblemDetails>(url, request, HttpStatusCode.BadRequest);

    protected async Task PostAsyncAndEnsureStatus<TRequest>(
        string url,
        TRequest request,
        HttpStatusCode statusCode)
    {
        var response = await PostAsync(url, request);
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);
        response.StatusCode.Should().Be(statusCode);
    }

    private async Task<TResult?> PostAsyncAndEnsureStatus<TRequest, TResult>(
        string url,
        TRequest request,
        HttpStatusCode statusCode)
    {
        var response = await PostAsync(url, request);
        var content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(statusCode);

        return JsonConvert.DeserializeObject<TResult>(content);
    }

    private async Task<HttpResponseMessage> PostAsync<TRequest>(string url, TRequest request)
    {
        var json = JsonConvert.SerializeObject(request);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, stringContent);

        return response;
    }

    private async Task<TResult?> GetAsyncAndEnsureStatus<TResult>(string url, HttpStatusCode statusCode)
    {
        var response = await _httpClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(statusCode);

        return JsonConvert.DeserializeObject<TResult>(content);
    }

    private async Task<TResult?> DeleteAsyncAndEnsureStatus<TResult>(string url, HttpStatusCode statusCode)
    {
        var response = await DeleteAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(statusCode);

        return JsonConvert.DeserializeObject<TResult>(content);
    }

    private async Task DeleteAsyncAndEnsureStatus(string url, HttpStatusCode statusCode)
    {
        var response = await DeleteAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);
        response.StatusCode.Should().Be(statusCode);
    }

    private async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        var response = await _httpClient.DeleteAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);
        return response;
    }

    private async Task<TResult?> PutAsyncAndEnsureStatus<TRequest, TResult>(
        string url,
        TRequest request,
        HttpStatusCode statusCode)
    {
        var response = await PutAsync(url, request);
        var content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(statusCode);

        return JsonConvert.DeserializeObject<TResult>(content);
    }

    private async Task PutAsyncAndEnsureStatus<TRequest>(
        string url,
        TRequest request,
        HttpStatusCode statusCode)
    {
        var response = await PutAsync(url, request);
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);
        response.StatusCode.Should().Be(statusCode);
    }

    private async Task<HttpResponseMessage> PutAsync<TRequest>(string url, TRequest request)
    {
        var json = JsonConvert.SerializeObject(request);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(url, stringContent);

        return response;
    }

    private async Task<TResult?> PatchAsyncAndEnsureStatus<TRequest, TResult>(
        string url,
        TRequest request,
        HttpStatusCode statusCode)
    {
        var response = await PatchAsync(url, request);
        var content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(statusCode);

        return JsonConvert.DeserializeObject<TResult>(content);
    }

    private async Task PatchAsyncAndEnsureStatus<TRequest>(
        string url,
        TRequest request,
        HttpStatusCode statusCode)
    {
        var response = await PatchAsync(url, request);
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);
        response.StatusCode.Should().Be(statusCode);
    }

    private async Task<HttpResponseMessage> PatchAsync<TRequest>(string url, TRequest request)
    {
        var json = JsonConvert.SerializeObject(request);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PatchAsync(url, stringContent);

        return response;
    }
}
