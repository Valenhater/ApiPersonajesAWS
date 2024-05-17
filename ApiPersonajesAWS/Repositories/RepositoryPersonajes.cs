using ApiPersonajesAWS.Data;
using ApiPersonajesAWS.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;

namespace ApiPersonajesAWS.Repositories
{
    public class RepositoryPersonajes
    {
        private PersonajesContext context;

        public RepositoryPersonajes(PersonajesContext context)
        {
            this.context = context;
        }

        public async Task<List<Personaje>> GetPersonajesAsync()
        {
            return await this.context.Personajes.ToListAsync();
        }
        public async Task<Personaje> FindPersonajeAsync(int id)
        {
            return await this.context.Personajes.FirstOrDefaultAsync( x=> x.IdPersonaje == id);
        }
        private async Task<int> GetMaxIdPersonajeAsync()
        {
            return await this.context.Personajes.MaxAsync(x => x.IdPersonaje) +1;
        }

        public async Task CreatePersonajeAsync(string nombre, string imagen)
        {
            Personaje personaje = new Personaje
            {
                IdPersonaje = await this.GetMaxIdPersonajeAsync(),
                Nombre = nombre,
                Imagen = imagen
            };
            this.context.Personajes.Add(personaje);
            await this.context.SaveChangesAsync();  
        }
        public async Task UpdatePersonajeAsync(int idPersonaje, string nombre, string imagen)
        {
            using (var connection = this.context.Database.GetDbConnection() as MySqlConnection)
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("UpdatePersonaje", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_IDPERSONAJE", idPersonaje);
                    command.Parameters.AddWithValue("p_PERSONAJE", nombre);
                    command.Parameters.AddWithValue("p_IMAGEN", imagen);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
