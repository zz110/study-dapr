using BackEnd;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#if DEBUG

//¿ª·¢µ÷ÊÔ
Process[] processes = System.Diagnostics.Process.GetProcessesByName("daprd");

foreach (Process process in processes)
{
    string argss = process.GetCommandLineArgs();
}

if (processes.Any(x => x.GetCommandLineArgs().Contains(" backend ")) == false)
{
    Process.Start(@"C:\Users\xiaocai\.dapr\bin\daprd.exe", "--app-id backend --app-port 5000 -dapr-http-port 50000 -dapr-grpc-port 30000 -metrics-port 9002");
}


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
