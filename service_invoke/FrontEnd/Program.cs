using System.Diagnostics;
using Dapr.Client;
using Dapr.Extensions.Configuration;
using FrontEnd;
using FrontEnd.ActorDefine;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region �������


#if DEBUG

//��������
//Process[] processes = System.Diagnostics.Process.GetProcessesByName("daprd");

//foreach (Process process in processes)
//{
//    string argss = process.GetCommandLineArgs();
//}

//if (processes.Any(x => x.GetCommandLineArgs().Contains(" frontend ")) == false)
//{
//    Process.Start(@"C:\Users\xiaocai\.dapr\bin\daprd.exe", "--app-id frontend --app-port 5001 -dapr-http-port 50001 -dapr-grpc-port 30001 -metrics-port 9002");
//}


//������ע��DaprClient ʵ��ʱ�Ż��õ��˴�����
//builder.Services.AddControllers().AddDapr(config =>
//{
//    //��˷���dapr�˵�
//    config.UseHttpEndpoint("http://localhost:3501");
//    //config.UseGrpcEndpoint("http://localhost:30000");
//});

#else

 builder.Services.AddControllers().AddDapr();

#endif

#endregion


builder.Services.AddLogging();
builder.Services.AddControllers().AddDapr();
builder.Services.AddActors(opt =>
{
    opt.Actors.RegisterActor<OrderStatusActor>();
});

builder.Host.ConfigureAppConfiguration(config =>
{
    var daprClient = new DaprClientBuilder().Build();
    var secretDescriptors = new List<DaprSecretDescriptor> { new DaprSecretDescriptor("RabbitMQConnectStr") };
    config.AddDaprSecretStore("secrets01", secretDescriptors, daprClient);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.UseKestrel().UseUrls("http://localhost:5001");

var app = builder.Build();


app.Use((context, next) =>
{
    context.Request.EnableBuffering();
    return next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.UseCloudEvents();
app.MapSubscribeHandler();
app.MapActorsHandlers();

app.MapControllers();


app.Run();
