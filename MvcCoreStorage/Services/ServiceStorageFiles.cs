using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using System.ComponentModel;

namespace MvcCoreStorage.Services
{
    public class ServiceStorageFiles
    {
        //TODO SERVICIO AZURE STORAGE FUNCIONA MEDIANTE UN 
        //CLIENT.  A PARTIR DE AHI, ES CUANDO GENERAMOS LAS 
        //FUNCIONALIDADES
        //ESTE OBJETO ES EL DIRECTORIO RAIZ
        private ShareDirectoryClient root;

        public ServiceStorageFiles(IConfiguration configuration)
        {
            //CREAMOS AQUI EL CLIENTE DE FORMA EXPLICITA
            string azureKeys =
                configuration.GetValue<string>("AzureKeys:StorageAccount");
            ShareClient shareClient =
                new ShareClient(azureKeys, "ejemplofiles");
            this.root = shareClient.GetRootDirectoryClient();
        }

        //METODO PARA RECUPERAR TODOS LOS FILES 
        //DEVOLVEMOS SU NAME
        public async Task<List<string>> GetFilesAsync()
        {
            List<string> files = new List<string>();
            //RECORREMOS TODOS LOS FICHEROS DE LA RAIZ
            await foreach (ShareFileItem item in
                this.root.GetFilesAndDirectoriesAsync())
            {
                files.Add(item.Name);
            }
            return files;
        }

        //METODO PARA LEER UN FICHERO
        public async Task<string> ReadFileAsync(string fileName)
        {
            //PARA ACCEDER A UN FILE NECESITO QUE UN DIRECTORIO
            //ME OFREZCA EL FILE
            ShareFileClient file = this.root.GetFileClient(fileName);
            ShareFileDownloadInfo data = await file.DownloadAsync();
            Stream stream = data.Content;
            string contenido = "";
            using (StreamReader reader = new StreamReader(stream))
            {
                contenido = await reader.ReadToEndAsync();
            }
            return contenido;
        }

        //METODO PARA SUBIR FICHEROS
        public async Task UploadFileAsync(string fileName, 
            Stream stream)
        {
            ShareFileClient file = this.root.GetFileClient(fileName);
            //CREAMOS EL FICHERO, ESTO ES OPCIONAL, PERO SI LE DECIMOS
            //EL TAMAÑO IRA MEJOR
            await file.CreateAsync(stream.Length);
            await file.UploadAsync(stream);
        }

        //METODO PARA ELIMINAR FICHEROS
        public async Task DeleteFilesAsync(string fileName)
        {
            ShareFileClient file = this.root.GetFileClient(fileName);
            await file.DeleteAsync();
        }
    }
}
