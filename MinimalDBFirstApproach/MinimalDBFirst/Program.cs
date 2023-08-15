using MiniDB0731.EndPoints;
using MinimalDBFirst.AutoMapper;
using MinimalDBFirst.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DotnetContext>();
builder.Services.AddAutoMapper(typeof(AutoMappingConfig));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UsersEndPoint();

app.Run();
