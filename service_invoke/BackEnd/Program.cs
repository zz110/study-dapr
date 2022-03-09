 var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#if DEBUG

builder.Services.AddControllers().AddDapr(config =>
{
    config.UseHttpEndpoint("http://localhost:50000");
    config.UseGrpcEndpoint("http://localhost:30000");
});

#else

 builder.Services.AddControllers().AddDapr();

#endif


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.UseKestrel().UseUrls("http://*:5000");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
