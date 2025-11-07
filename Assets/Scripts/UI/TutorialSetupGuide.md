# Hướng Dẫn Setup Tutorial Panel

## Mục tiêu:
- Back button ở góc trên bên phải, dùng sprite `button.png`
- Nội dung hướng dẫn trong bảng với sprite `table.png`
- Hướng dẫn cách chơi game Light of Kindness

## Các bước thực hiện trong Unity Editor:

### 1. Mở Scene MainMenu
- Trong Hierarchy, tìm **Canvas → TutorialPanel**
- Đảm bảo TutorialPanel đang INACTIVE (unchecked)

### 2. Xóa hoặc Ẩn Title cũ
- Tìm **TutorialPanel → SettingsTitle** (text "Tutorial" ở trên)
- Xóa hoặc disable GameObject này (không cần nữa)

### 3. Cập nhật TutorialBackButton (di chuyển lên góc trên phải)
- Chọn **TutorialPanel → TutorialBackButton**
- **RectTransform**:
  - Anchor Preset: Click góc trái trên, giữ Alt+Shift, chọn góc phải trên (top-right)
  - Anchored Position X: **-80**
  - Anchored Position Y: **-60**
  - Width: **200**, Height: **80**
  
- **Image Component**:
  - Source Image: Kéo `button.png` từ `Assets/Sprites/UI`
  - Image Type: Simple
  - Color: White hoặc màu bạn thích

- **Text (TMP)** (child):
  - Text: `X` hoặc `Back` hoặc `←`
  - Font: `Gixel SDF`
  - Font Size: 40
  - Color: White
  - Alignment: Center + Middle

### 4. Tạo Content Table (Bảng chứa nội dung)
1. Right-click **TutorialPanel** → UI → Image
2. Đổi tên thành **ContentTable**
3. **RectTransform**:
   - Anchor: Center-Middle
   - Pos X: 0, Pos Y: 0
   - Width: 1200, Height: 700
   
4. **Image Component**:
   - Source Image: Kéo `table.png` từ `Assets/Sprites/UI`
   - Image Type: Sliced (nếu có border) hoặc Simple
   - Color: White

### 5. Tạo Tutorial Content Text
1. Right-click **ContentTable** → UI → Text - TextMeshPro
2. Đổi tên thành **TutorialText**
3. **RectTransform**:
   - Anchor: Stretch Stretch (full)
   - Left: 80, Top: 80, Right: 80, Bottom: 80 (padding)
   
4. **TextMeshPro Component**:
   - Font: `Gixel SDF`
   - Font Size: 28
   - Color: Black hoặc Dark Gray (dễ đọc trên nền sáng)
   - Alignment: Left + Top
   - Wrapping: Enabled
   - Overflow: Scroll hoặc Truncate

### 6. Nội Dung Tutorial (Vietnamese)
Copy và paste vào **TutorialText**:

```
=== LIGHT OF KINDNESS ===
Hướng Dẫn Chơi Game

MỤC TIÊU:
• Giải cứu các nạn nhân bị bắt giữ
• Tiêu diệt quái vật và sống sót
• Thu thập vàng và vật phẩm

ĐIỀU KHIỂN:
• WASD hoặc Phím mũi tên: Di chuyển
• Chuột: Nhắm và tấn công
• Số 1-3: Chọn vũ khí/công cụ
  - 1: Đèn pin (chiếu sáng)
  - 2: Khiên (phòng thủ)
  - 3: Vũ khí tấn công
• Space: Lăn né (dash)

VŨ KHÍ & CÔNG CỤ:
• Đèn Pin: Chiếu sáng khu vực tối
• Khiên: Kích hoạt để bảo vệ
• Kiếm, Cung, Gậy: Tấn công kẻ địch

CHỈ SỐ:
• Máu (Trái tim): Sinh lực của bạn
• Stamina (Màu xanh): Năng lượng
• Pin (Battery): Năng lượng đèn pin

VẬT PHẨM:
• Trái tim: Hồi máu
• Đồng xu vàng: Thu thập điểm
• Vũ khí: Nâng cấp sức mạnh

MẸO CHƠI:
✓ Dùng đèn pin để thấy rõ trong bóng tối
✓ Khiên giúp bạn chặn đòn tấn công
✓ Né tránh khi hết stamina
✓ Thu thập vàng để nâng cấp
✓ Giải cứu nạn nhân để hoàn thành màn

Chúc bạn may mắn!
```

### 7. (Tùy chọn) Thêm Scroll View
Nếu nội dung quá dài:
1. Chuyển **TutorialText** thành child của một **Scroll View**
2. Right-click **ContentTable** → UI → Scroll View
3. Di chuyển **TutorialText** vào **Scroll View → Viewport → Content**

### 8. Test
- Active TutorialPanel trong Scene view để xem preview
- Kiểm tra button Back có hoạt động không
- Đảm bảo text dễ đọc và phù hợp với bảng

## Cấu Trúc Hierarchy Cuối Cùng:
```
TutorialPanel
├── TutorialBackButton (top-right corner)
│   └── Text (TMP)
└── ContentTable (Image with table.png)
    └── TutorialText (TextMeshPro)
```

## Ghi chú:
- Có thể thêm icon cho các phím điều khiển
- Có thể thêm hình ảnh minh họa
- Điều chỉnh font size nếu cần
- Có thể làm animation cho text

