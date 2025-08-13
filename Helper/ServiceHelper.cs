namespace OrderApp.Helper
{
    public static class ServiceHelper
    {
        public static T Resolve<T>() where T : notnull
        {
            try
            {
                return App.Services.GetRequiredService<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
