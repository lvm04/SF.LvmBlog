namespace SF.LvmBlog.ViewModels;

public class SortViewModel
{
    public SortState Current { get; set; }          // значение свойства, выбранного для сортировки
    public bool Up { get; set; }                    // Сортировка по возрастанию или убыванию

    public SortViewModel(SortState sortOrder, bool up)
    {
        Current = sortOrder;
        Up = up;
    }
}
