namespace Lurker.AppData.Console
{
    public class ExampleFile : AppDataFileBase<Model>
    {
        protected override string FileName => "Example";

        protected override string FolderName => "Example";
    }
}
