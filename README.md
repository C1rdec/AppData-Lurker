# Example

```c#
var file = new FileExample();
file.Initialize();
```

### Definitions
```c#
public class FileExample : AppDataFileBase<ModelExample>
{
    /// <summary>
    /// The name of the saved file.
    /// </summary>
    protected override string FileName => "Example.json";

    /// <summary>
    /// The name of the root folder in the AppData.
    /// </summary>
    protected override string FolderName => "Example";

    /// <summary>
    /// *Optional* The name of the sub folder.
    /// </summary>
    protected override string SubFolderName => "SubFolder";
}
```

```c#
public class ModelExample
{
    public int Value { get; set; }
}
```

### Result

![image](https://user-images.githubusercontent.com/5436436/153727110-fc9577b5-0a43-422f-aa1d-07a30bb86687.png)
