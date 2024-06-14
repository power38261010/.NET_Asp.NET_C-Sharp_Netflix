using MercadoPago.Config;

namespace NetflixClone.Config {

    public static class MercadoPagoConfigs {
        public static void Initialize( IConfiguration configuration ) {
            // ArgumentNullException.ThrowIfNull(configuration);

            MercadoPagoConfig.AccessToken = configuration["MercadoPago:AccessToken"];
            // MercadoPagoConfig.Sandbox = true;
        }
    }

}