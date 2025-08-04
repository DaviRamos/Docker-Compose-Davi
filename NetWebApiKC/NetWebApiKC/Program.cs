using Microsoft.OpenApi.Models;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authentication;
using Keycloak.AuthServices.Sdk;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;    
using System.Security.Claims;
using NetWebApiKC;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddAuthorizationBuilder();
builder.Services.AddTransient<IClaimsTransformation, RoleClaimsTransformation>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "NetWebApiKC",
        Description = "Teste de Autenticação e Autorização com Keycloak",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
     options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}' here",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
            },
            new List<string>() }
    });
});

//builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
//builder.Services.AddKeycloakAuthorization(builder.Configuration);

/*
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
builder.Services.AddKeycloakAuthorization(builder.Configuration);
*/

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddKeycloakWebApi(builder.Configuration);
builder.Services .AddAuthorization().AddKeycloakAuthorization().AddAuthorizationBuilder().AddPolicy(
    "RequireWritedataRole",
    policy => policy.RequireResourceRolesForClient(
        "netwebapikc-api", // client conforme configurado no Keycloak
        ["administrador"] // Roles necessárias (Nome da role definida lá no Keycloak)
    )
);


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//  app.UseSwagger();
//app.UseSwaggerUI();
//}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();