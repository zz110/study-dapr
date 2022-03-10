using System.Diagnostics;
using FrontEnd;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#if DEBUG

//开发调试
Process[] processes = System.Diagnostics.Process.GetProcessesByName("daprd");

foreach (Process process in processes)
{
    string argss = process.GetCommandLineArgs();
}

if (processes.Any(x => x.GetCommandLineArgs().Contains(" frontend ")) == false)
{
    Process.Start(@"C:\Users\xiaocai\.dapr\bin\daprd.exe", "--app-id frontend --app-port 5001 -dapr-http-port 50001 -dapr-grpc-port 30001 -metrics-port 9002");
}


//仅依赖注入DaprClient 实例时才会用到此处配置
builder.Services.AddControllers().AddDapr(config =>
{
    //后端服务dapr端点
    config.UseHttpEndpoint("http://localhost:3511");
    //config.UseGrpcEndpoint("http://localhost:30000");
});

#else

 builder.Services.AddControllers().AddDapr();

#endif




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.UseKestrel().UseUrls("http://*:5001");

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
