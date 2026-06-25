
public class EnrollmentWorker
{
    private readonly IServiceScopeFactory factory;

    public EnrollmentWorker(IServiceScopeFactory factory)
    {
        this.factory = factory;
    }

    public async Task RunAsync()
    {
        using var scope = factory.CreateScope();

        var svc = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();

        // Call via dynamic to avoid compile-time dependency on a specific method name
        await ((dynamic)svc).EnrollmentRecordAsync();
    }
}