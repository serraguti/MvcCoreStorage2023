using Azure;
using Azure.Data.Tables;
using MvcCoreStorage.Models;

namespace MvcCoreStorage.Services
{
    public class ServiceStorageTables
    {
        private TableClient tableClient;

        public ServiceStorageTables(TableServiceClient tableService)
        {
            //SOLAMENTE LO VAMOS A UTILIZAR PARA RECUPERAR NUESTRA TABLA
            //SI LA TABLA NO EXISTE, LA CREAMOS
            this.tableClient = tableService.GetTableClient("clientes");
            //CREAMOS LA TABLA SI NO EXISTE EN AZURE
            Task.Run(async () => {
                await this.tableClient.CreateIfNotExistsAsync();
            });
        }

        public async Task 
            CreateClientAsync(int id, string nombre, int salario
            
            , int edad, string empresa)
        {
            Cliente cliente = new Cliente();
            cliente.IdCliente = id;
            cliente.Nombre = nombre;
            cliente.Salario = salario;
            cliente.Edad = edad;
            cliente.Empresa = empresa;
            await this.tableClient.AddEntityAsync<Cliente>(cliente);
        }

        //METODO PARA BUSCAR UN CLIENTE
        //EN LA BUSQUEDA PURA POR PK, NECESITAMOS LAS DOS CLAVES
        public async Task<Cliente> FindClienteAsync
            (string partitionKey, string rowKey)
        {
            Cliente cliente = await
                this.tableClient.GetEntityAsync<Cliente>
                (partitionKey, rowKey);
            return cliente;
        }

        //METODO PARA ELIMINAR OBJETOS DE LA TABLA
        public async Task DeleteClienteAsync
            (string partitionKey, string rowKey)
        {
            await this.tableClient.DeleteEntityAsync
                (partitionKey, rowKey);
        }

        //METODO PARA RECUPERAR TODOS LOS REGISTROS
        public async Task<List<Cliente>> GetClientesAsync()
        {
            List<Cliente> clientes = new List<Cliente>();
            var query =
                this.tableClient.QueryAsync<Cliente>
                (filter: "");
            await foreach (Cliente item in query)
            {
                clientes.Add(item);
            }
            return clientes;
        }

        public async Task<List<Cliente>> GetClientesEmpresaAsync
            (string empresa)
        {
            //var query = 
            //    this.tableClient.QueryAsync<Cliente>
            //    (filter: "Campo eq valor and Campo2 gt valor");
            //var query =
            //    this.tableClient.QueryAsync<Cliente>
            //    (filter: "Empresa eq " + empresa);
            var query =
                this.tableClient.Query<Cliente>
                (x => x.Empresa == empresa);
            //var query =
            //    this.tableClient.QueryAsync<Cliente>
            //    (x => x.Empresa == empresa);
            return query.ToList();
        }
    }
}
