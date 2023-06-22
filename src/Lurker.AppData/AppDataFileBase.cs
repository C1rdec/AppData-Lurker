using System;
using System.IO;
using System.Text.Json;

namespace Lurker.AppData
{
    public abstract class AppDataFileBase<TEntity>
        where TEntity : class, new()
    {
        #region Constructors

        public AppDataFileBase()
        {
            Entity = new TEntity();
        }

        #endregion

        #region Methods

        public virtual void Initialize()
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            HandleSubFolder();

            if (!File.Exists(this.FilePath))
            {
                Save();
            }
            else
            {
                try
                {
                    Entity = JsonSerializer.Deserialize<TEntity>(File.ReadAllText(FilePath));
                }
                catch
                {
                    var backupPath = $"{FilePath}.bak";
                    if (File.Exists(backupPath))
                    {
                        File.Delete(backupPath);
                    }

                    File.Move(FilePath, backupPath);
                    
                    Entity = new TEntity();
                    Save();
                }
            }
        }

        #endregion

        #region Events

        public event EventHandler<TEntity> OnFileSaved;

        #endregion

        #region Properties

        public TEntity Entity;

        protected abstract string FileName { get; }

        protected abstract string FolderName { get; }

        protected virtual string SubFolderName => string.Empty;

        protected virtual string ImportFileExtension => ".txt";

        protected string FilePath => Path.Combine(FolderPath, FileName);

        protected string FolderPath => Path.Combine(AppDataFolderPath, FolderName, SubFolderName?.Trim() ?? string.Empty);

        private string AppDataFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        #endregion

        #region Methods

        public void Delete()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }

            Entity = new TEntity();
        }

        public void Save() => Save(Entity);

        public void Save(bool raiseEvent) => Save(Entity, raiseEvent);

        public void Save(string jsonValue) => Save(Deserialize<TEntity>(jsonValue));

        public void Save(string jsonValue, bool raiseEvent) => Save(Deserialize<TEntity>(jsonValue), raiseEvent);

        public void Save(TEntity entity) => Save(entity, true);

        public void Save(TEntity entity, bool raiseEvent)
        {
            var name = string.Empty;
            var nameProperty = entity.GetType().GetProperty("Name");
            if (nameProperty != null)
            {
                var oName = nameProperty.GetValue(entity);
                name = oName as string;
            }

            var filePath = string.IsNullOrEmpty(name) ? FilePath : Path.Combine(FolderPath, name);

            HandleSubFolder();
            var jsonValue = JsonSerializer.Serialize(entity, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonValue);

            Entity = entity;

            if (raiseEvent)
            {
                OnFileSaved?.Invoke(this, entity);
            }
        }

        protected T Deserialize<T>() => Deserialize<T>(File.ReadAllText(this.FilePath));

        protected T Deserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new InvalidOperationException("The json value must not be null");
            }

            return JsonSerializer.Deserialize<T>(json);
        }

        private void HandleSubFolder()
        {
            if (string.IsNullOrEmpty(SubFolderName))
            {
                return;
            }

            var path = Path.Combine(FolderPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        #endregion
    }
}