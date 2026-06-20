using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio
{
    public class GreenApiWhatsApp
    {
        private static readonly string IdInstance = "7107620140";
        private static readonly string ApiToken = "91939fd2915c46f08ef525274512da7fcc7214f642b74bed82";

        // Aquí solo cambias el link si Green API cambia
        private static readonly string UrlBase = "https://7107.api.greenapi.com";

        public static async Task<bool> EnviarMensajeAsync(string celular, string mensaje)
        {
            try
            {
                celular = FormatearNumeroEcuador(celular);

                string url = $"{UrlBase}/waInstance{IdInstance}/sendMessage/{ApiToken}";

                string json = @"{
                    ""chatId"": """ + celular + @"@c.us"",
                    ""message"": """ + mensaje.Replace("\"", "\\\"") + @"""
                }";

                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        private static string FormatearNumeroEcuador(string celular)
        {
            celular = celular.Trim();

            // 0999999999 -> 593999999999
            if (celular.StartsWith("0"))
                celular = "593" + celular.Substring(1);

            return celular;
        }
    }
}