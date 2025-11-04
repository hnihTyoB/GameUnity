# Shadow Spawn System - Quick Start (Tiếng Việt)

## Setup Nhanh trong 5 Phút ⚡

### Cách 1: Tự Động (Khuyên Dùng)

1. **Mở Unity Editor**
2. **Menu:** `GameObject → Shadow Spawn → Setup Complete System`
3. **Kết quả:** Tự động tạo 1 Manager + 3 Spawn Zones
4. **Di chuyển** các Spawn Zones đến vị trí mong muốn trong map
5. **Select** Shadow Spawn Manager trong Hierarchy
6. **Assign Prefabs:**
   - Shadow Ghost 1 Prefab → Kéo prefab `Shadow Ghost` vào
   - Shadow Ghost 2 Prefab → Kéo prefab `Shadow Ghost 2` vào
   - Shadow Ghost 3 Prefab → Kéo prefab `Shadow Ghost 3` vào
7. **Select** từng Shadow Spawn Zone
8. **Set Obstacle Layer:** Chọn layers của tường/obstacles (Ground, Wall, v.v.)
9. **Play** và test!

✅ **Xong!**

---

### Cách 2: Thủ Công

#### Bước 1: Tạo Spawn Zones

```
1. Menu: GameObject → Shadow Spawn → Create Spawn Zone
2. Đặt tên: "Shadow Spawn Zone 1"
3. Di chuyển đến vị trí muốn spawn shadows
4. Inspector → Set "Obstacle Layer" (chọn Ground, Wall, etc.)
5. Lặp lại cho các zones khác
```

#### Bước 2: Tạo Manager

```
1. Menu: GameObject → Shadow Spawn → Create Spawn Manager
2. Inspector → Assign Shadow Prefabs (Ghost 1, 2, 3)
3. Assign Spawn Zones (kéo các zones vào array)
4. Check "Auto Spawn" ✓
```

✅ **Xong!**

---

## Settings Cơ Bản

### Shadow Spawn Manager

| Setting | Giá Trị Đề Xuất | Mô Tả |
|---------|-----------------|-------|
| **Max Shadows In Scene** | 10 | Tối đa shadow trong map |
| **Spawn Interval** | 15s | Thời gian giữa spawns |
| **Initial Spawn Delay** | 5s | Delay trước spawn đầu |
| **Shadow Ghost 1 Weight** | 60 | 60% spawn Ghost 1 |
| **Shadow Ghost 2 Weight** | 30 | 30% spawn Ghost 2 |
| **Shadow Ghost 3 Weight** | 10 | 10% spawn Ghost 3 |
| **Auto Spawn** | ✓ | Tự động spawn |

### Shadow Spawn Zone

| Setting | Giá Trị Đề Xuất | Mô Tả |
|---------|-----------------|-------|
| **Zone Size** | (10, 10) | Kích thước vùng spawn |
| **Obstacle Layer** | Ground, Wall | Layers cần tránh |
| **Min Distance From Player** | 5 | Không spawn gần player |
| **Spawn Check Radius** | 0.5 | Bán kính check va chạm |

---

## Test Trong Play Mode

### Inspector Buttons (Shadow Spawn Manager)

1. **Play** game
2. **Select** Shadow Spawn Manager
3. **Inspector** sẽ hiện buttons:
   - `Spawn Shadow Now` → Spawn 1 shadow ngay
   - `Start Auto Spawn` → Bật auto spawn
   - `Stop Auto Spawn` → Tắt auto spawn
   - `Destroy All Shadows` → Xóa tất cả shadows

### Debug Info
- **Active Shadows:** Số shadow hiện tại
- **Can Spawn More:** Có thể spawn thêm không

---

## Visualizations Trong Scene View

### Khi Select Spawn Zone:
- 🟪 **Hộp tím:** Vùng spawn
- 🟨 **Vòng vàng:** Khoảng cách min từ player
- 🟩 **Vòng xanh:** Vị trí spawn mẫu

---

## Troubleshooting

### ❌ Shadow không spawn?
1. ✅ Check prefabs đã assign chưa
2. ✅ Check spawn zones có active không
3. ✅ Check Obstacle Layer đã set chưa
4. ✅ Check Console có lỗi không

### ❌ Shadow spawn vào tường?
1. ✅ Set Obstacle Layer trong Spawn Zone
2. ✅ Đảm bảo tường có đúng Layer
3. ✅ Tăng Spawn Check Radius

### ❌ Shadow spawn quá gần player?
1. ✅ Tăng Min Distance From Player

---

## Gọi Từ Code (Optional)

```csharp
// Lấy reference
ShadowSpawnManager manager = FindObjectOfType<ShadowSpawnManager>();

// Spawn ngay
manager.SpawnRandomShadow();

// Start/Stop
manager.StartSpawning();
manager.StopSpawning();

// Check status
int count = manager.GetActiveShadowCount();
bool canSpawn = manager.CanSpawnMore();

// Destroy all
manager.DestroyAllShadows();
```

---

## Ví Dụ Use Cases

### Use Case 1: Normal Gameplay
```
Settings:
- Max Shadows: 8
- Spawn Interval: 20s
- Weights: 70/20/10
→ Moderate difficulty, balanced
```

### Use Case 2: Boss Fight
```
Settings:
- Max Shadows: 15
- Spawn Interval: 10s  
- Weights: 40/40/20
→ High difficulty, intense
```

### Use Case 3: Safe Zone
```
Settings:
- Auto Spawn: OFF
- Manual spawn khi cần
→ Controlled spawning
```

---

## Checklist Hoàn Thành

- [ ] Tạo Shadow Spawn Manager
- [ ] Tạo ít nhất 1 Spawn Zone
- [ ] Assign Shadow Prefabs
- [ ] Set Obstacle Layer cho zones
- [ ] Di chuyển zones đến vị trí đúng
- [ ] Test spawn trong Play mode
- [ ] Kiểm tra shadows không spawn vào tường
- [ ] Kiểm tra không spawn quá gần player
- [ ] Adjust settings cho phù hợp
- [ ] Test performance

---

## Files Tham Khảo

📄 **Chi Tiết:** `SHADOW_SPAWN_SETUP_GUIDE.md`  
📄 **Kỹ Thuật:** `SHADOW_SPAWN_FEATURES.md`

---

**Chúc bạn thành công! 🎮**

