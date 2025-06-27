using System.Collections.ObjectModel;
using System.Linq;
using AngleSharp.Common;
using CommunityToolkit.Mvvm.Input;
using EasyTemplate.Ava.Common;
using EasyTemplate.Ava.Tool.Util;

namespace EasyTemplate.Ava.Features;

public partial class DataViewModel : ViewModelBase
{
    public DataViewModel()
    {
        // 示例数据填充
        for (int i = 1; i <= 53; i++)
        {
            AllInvoices.Add(new Invoice
            {
                Id = i,
                BillingName = $"客户{i}",
                AmountPaid = i * 100,
                Paid = i % 2 == 0
            });
        }
        UpdatePagination();

        ActionBase.SignOutAction = show =>
        {
            AllInvoices.Clear();
            // 示例数据填充
            for (int i = 1; i <= 500; i++)
            {
                AllInvoices.Add(new Invoice
                {
                    Id = i,
                    BillingName = $"测试{i}",
                    AmountPaid = i * 100,
                    Paid = i % 2 == 0
                });
            }
            UpdatePagination();
        };

        ChangeLanguage(Setting.Config.Application.Language);
        ActionBase.ChangeLanguageAction += ChangeLanguage;
    }

    [RelayCommand]
    private void FirstPage()
    {
        if (CurrentPage != 1)
        {
            CurrentPage = 1;
            UpdatePagination();
        }
    }

    [RelayCommand]
    private void PrevPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            UpdatePagination();
        }
    }

    [RelayCommand]
    private void NextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            UpdatePagination();
        }
    }

    [RelayCommand]
    private void LastPage()
    {
        if (CurrentPage != TotalPages)
        {
            CurrentPage = TotalPages;
            UpdatePagination();
        }
    }

    private void UpdatePagination()
    {
        IsLoading = true;

        TotalPages = (AllInvoices.Count + PageSize - 1) / PageSize;
        var pageData = AllInvoices.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        Invoices = [.. pageData];

        IsLoading = false;
    }

    [RelayCommand]
    private void Edit(Invoice? invoice)
    {
        if (invoice is null) return;
        // 编辑逻辑

        var res = AvaBase.DialogManager.CreateDialog()
            .WithViewModel(dialog => new DataAddOrUpdateDialogViewModel(dialog, invoice))
            .Dismiss()
            .ByClickingBackground()
            .TryShow();
        if (res)
        {
            UpdatePagination();
        }
    }

    [RelayCommand]
    private void View(Invoice? invoice)
    {
        if (invoice is null) return;
        // 查看逻辑
    }

    [RelayCommand]
    private void Delete(Invoice invoice)
    {
        if (invoice is null) return;

        // 删除逻辑
        AllInvoices.Remove(invoice);
        UpdatePagination();
    }

    [RelayCommand]
    private void Add()
    {
        var dialog = AvaBase.DialogManager.CreateDialog()
            .WithViewModel(dialog => new DataAddOrUpdateDialogViewModel(dialog, null));
        dialog.Dismiss()
            .ByClickingBackground()
            .TryShow();
        dialog.Dialog.OnDismissed += (sender) =>
        {
            // 处理对话框关闭后的逻辑
            UpdatePagination();
        };
        //var res = AvaBase.DialogManager.CreateDialog()
        //    .WithViewModel(dialog => new DataAddOrUpdateDialogViewModel(dialog, null))
        //    .Dismiss()
        //    .ByClickingBackground()
        //    .TryShow();
        //if (res)
        //{
        //    UpdatePagination();
        //}
    }

    [RelayCommand]
    private void Reset()
    {
        // 重置逻辑
        CurrentPage = 1;
        PageSize = DefaultPageSize;
        UpdatePagination();
    }

    [RelayCommand]
    private void Search()
    {
        var keyword1 = Keyword;
        CurrentPage = 1;
        PageSize = DefaultPageSize;
        UpdatePagination();
    }

    private void ChangeLanguage(string lang)
    {
        AddContent = Localization.Get("add");
        ResetContent = Localization.Get("reset");
        KeywordContent = Localization.Get("keyword");
        ActionContent = Localization.Get("action");
        EditContent = Localization.Get("edit");
        CheckContent = Localization.Get("check");
        DeleteContent = Localization.Get("delete");
        LoadingContent = Localization.Get("loading");
    }

    //public IAvaloniaReadOnlyList<InvoiceViewModel> Invoices { get; } = new AvaloniaList<InvoiceViewModel>()
    //{
    //    new(15364, "Jean", 156, true),
    //    new(45689, "Fantine", 82, false),
    //    new(15364, "Jean", 156, true),
    //    new(45689, "Fantine", 82, false),
    //    new(15364, "Jean", 156, true),
    //    new(45689, "Fantine", 82, false),
    //};
    private const int DefaultPageSize = 10;

    // 全部数据
    public ObservableCollection<Invoice> AllInvoices { get; } = new();

    // 当前页数据
    [ObservableProperty]
    private ObservableCollection<Invoice> invoices = new();

    [ObservableProperty]
    private int currentPage = 1;

    [ObservableProperty]
    private int pageSize = DefaultPageSize;

    [ObservableProperty]
    private int totalPages = 1;
    [ObservableProperty] private bool isLoading = false;
    [ObservableProperty] private string keyword;
    [ObservableProperty] private string addContent;
    [ObservableProperty] private string resetContent;
    [ObservableProperty] private string keywordContent;
    [ObservableProperty] private string actionContent;
    [ObservableProperty] private string editContent;
    [ObservableProperty] private string checkContent;
    [ObservableProperty] private string deleteContent;
    [ObservableProperty] private string loadingContent;

}

// 示例实体
public class Invoice
{
    public int Id { get; set; }
    public string BillingName { get; set; }
    public decimal AmountPaid { get; set; }
    public bool Paid { get; set; }
}

public partial class InvoiceViewModel : ObservableObject
{
    [ObservableProperty] private int _id;
    [ObservableProperty] private string? _billingName;
    [ObservableProperty] private int _amountPaid;
    [ObservableProperty] private bool _paid;

    public InvoiceViewModel(int id, string? billingName, int amountPaid, bool paid)
    {
        Id = id;
        BillingName = billingName;
        AmountPaid = amountPaid;
        Paid = paid;
    }
}
