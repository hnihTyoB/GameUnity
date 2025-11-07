using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Quản lý phân trang cho Tutorial
/// Attach vào ContentTable GameObject
/// </summary>
public class TutorialPagination : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI pageIndicatorText; // Hiển thị "1/5"

    [Header("Tutorial Content Pages")]
    [SerializeField] [TextArea(15, 30)] private string[] tutorialPages;

    [Header("Settings")]
    [SerializeField] private bool loopPages = false; // Có quay vòng trang không?
    [SerializeField] private Color activeButtonColor = Color.white;
    [SerializeField] private Color disabledButtonColor = Color.gray;

    private int currentPage = 0;
    private int totalPages = 0;

    private void Start()
    {
        // Kiểm tra references
        if (tutorialText == null)
        {
            Debug.LogError("TutorialPagination: TutorialText chưa được gán!");
            return;
        }

        if (tutorialPages == null || tutorialPages.Length == 0)
        {
            Debug.LogError("TutorialPagination: Chưa có nội dung tutorial!");
            return;
        }

        totalPages = tutorialPages.Length;

        // Gán sự kiện cho buttons
        if (previousButton != null)
        {
            previousButton.onClick.AddListener(PreviousPage);
        }

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(NextPage);
        }

        // Hiển thị trang đầu tiên
        ShowPage(0);
    }

    private void ShowPage(int pageIndex)
    {
        // Kiểm tra index hợp lệ
        if (pageIndex < 0 || pageIndex >= totalPages)
        {
            Debug.LogWarning($"TutorialPagination: Page index {pageIndex} không hợp lệ!");
            return;
        }

        currentPage = pageIndex;

        // Cập nhật text
        tutorialText.text = tutorialPages[currentPage];

        // Cập nhật page indicator
        if (pageIndicatorText != null)
        {
            pageIndicatorText.text = $"{currentPage + 1}/{totalPages}";
        }

        // Cập nhật trạng thái buttons
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        // Previous button
        if (previousButton != null)
        {
            bool canGoPrevious = loopPages || currentPage > 0;
            previousButton.interactable = canGoPrevious;
            
            // Đổi màu button
            var colors = previousButton.colors;
            colors.normalColor = canGoPrevious ? activeButtonColor : disabledButtonColor;
            previousButton.colors = colors;
        }

        // Next button
        if (nextButton != null)
        {
            bool canGoNext = loopPages || currentPage < totalPages - 1;
            nextButton.interactable = canGoNext;
            
            // Đổi màu button
            var colors = nextButton.colors;
            colors.normalColor = canGoNext ? activeButtonColor : disabledButtonColor;
            nextButton.colors = colors;
        }
    }

    public void NextPage()
    {
        if (currentPage < totalPages - 1)
        {
            ShowPage(currentPage + 1);
        }
        else if (loopPages)
        {
            ShowPage(0); // Quay về trang đầu
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            ShowPage(currentPage - 1);
        }
        else if (loopPages)
        {
            ShowPage(totalPages - 1); // Quay về trang cuối
        }
    }

    // Method public để jump đến trang cụ thể
    public void GoToPage(int pageIndex)
    {
        ShowPage(pageIndex);
    }

    // Thêm trang mới runtime (nếu cần)
    public void AddPage(string content)
    {
        List<string> pages = new List<string>(tutorialPages);
        pages.Add(content);
        tutorialPages = pages.ToArray();
        totalPages = tutorialPages.Length;
        UpdateButtonStates();
    }
}

