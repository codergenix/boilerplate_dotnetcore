namespace DotNetCoreBoilerPlate.ViewModels
{
    public partial class DataTableJSONViewModel
    {
        public string order { get; set; }
        public string orderBy { get; set; }
        public int pageSize { get; set; }
        public string search { get; set; }
        public int skip { get; set; }
    }
}
