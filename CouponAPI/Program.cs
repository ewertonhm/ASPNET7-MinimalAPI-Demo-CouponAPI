using AutoMapper;
using CouponAPI;
using CouponAPI.Data;
using CouponAPI.Endpoints;
using CouponAPI.Models;
using CouponAPI.Models.Dto;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

var builder = WebApplication.CreateBuilder(args);


// Dependency Injection
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();


var app = builder.Build();

app.MapHealthChecks("/health");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureCouponEndpoints();
app.UseHttpsRedirection();
app.Run();
