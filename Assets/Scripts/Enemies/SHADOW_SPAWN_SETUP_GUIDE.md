# Hướng Dẫn Setup Shadow Spawn System

## Tổng Quan
Hệ thống spawn shadow ngẫu nhiên an toàn với kiểm tra va chạm và giới hạn số lượng.

## Các Components

### 1. ShadowSpawnZone
Định nghĩa vùng spawn cho shadows.

### 2. ShadowSpawnManager
Quản lý việc spawn shadows tự động hoặc theo lệnh.

---

## Cách Setup (Step-by-Step)

### Bước 1: Tạo Spawn Zones

1. Trong Scene, tạo Empty GameObject mới (Hierarchy → Right Click → Create Empty)
2. Đặt tên: `Shadow Spawn Zone 1`
3. Add Component → `ShadowSpawnZone`
4. Cấu hình trong Inspector:
   - **Zone Size**: Kích thước vùng spawn (VD: 10x10)
   - **Obstacle Layer**: Chọn layers của tường/obstacles (VD: "Ground", "Wall")
   - **Min Distance From Player**: Khoảng cách tối thiểu từ player (VD: 5)
   - **Spawn Check Radius**: Bán kính kiểm tra va chạm (VD: 0.5)

5. Di chuyển GameObject đến vị trí muốn spawn shadows
6. Trong Scene view, bạn sẽ thấy vùng spawn màu tím

**Lặp lại để tạo nhiều spawn zones ở các vị trí khác nhau trong map!**

### Bước 2: Tạo Spawn Manager

1. Tạo Empty GameObject mới
2. Đặt tên: `Shadow Spawn Manager`
3. Add Component → `ShadowSpawnManager`
4. Cấu hình trong Inspector:

   **Shadow Prefabs:**
   - Kéo prefab `Shadow Ghost` vào `Shadow Ghost 1 Prefab`
   - Kéo prefab `Shadow Ghost 2` vào `Shadow Ghost 2 Prefab`
   - Kéo prefab `Shadow Ghost 3` vào `Shadow Ghost 3 Prefab`
   
   **Spawn Settings:**
   - `Max Shadows In Scene`: Số shadow tối đa (VD: 10)
   - `Spawn Interval`: Thời gian giữa các lần spawn (VD: 15 giây)
   - `Initial Spawn Delay`: Delay trước khi spawn lần đầu (VD: 5 giây)
   - `Auto Spawn`: ✓ (Tự động spawn)
   
   **Shadow Type Weights:**
   - `Shadow Ghost 1 Weight`: 60 (60% cơ hội)
   - `Shadow Ghost 2 Weight`: 30 (30% cơ hội)
   - `Shadow Ghost 3 Weight`: 10 (10% cơ hội)
   
   **Spawn Zones:**
   - Kéo tất cả `Shadow Spawn Zone` objects vào array này
   - HOẶC để trống, script sẽ tự động tìm tất cả zones

   **Debug:**
   - `Show Debug Logs`: ✓ (Bật để xem logs)

### Bước 3: Setup Layers (QUAN TRỌNG!)

1. Edit → Project Settings → Tags and Layers
2. Tạo layer mới cho obstacles nếu chưa có (VD: "Ground", "Wall", "Obstacle")
3. Assign layer cho tất cả tường/obstacles trong scene
4. Quay lại `ShadowSpawnZone`, set `Obstacle Layer` = layers vừa tạo

---

## Ví Dụ Setup Cụ Thể

### Scenario 1: Map có 3 phòng
```
Phòng 1: Shadow Spawn Zone 1 (10x10)
Phòng 2: Shadow Spawn Zone 2 (8x12)
Phòng 3: Shadow Spawn Zone 3 (15x8)

Shadow Spawn Manager:
- Max Shadows: 6
- Spawn Interval: 20 giây
- Weights: 50/30/20 (Ghost1/Ghost2/Ghost3)
```

### Scenario 2: Boss Room với spawn liên tục
```
Boss Room: Shadow Spawn Zone Boss (20x20)

Shadow Spawn Manager:
- Max Shadows: 15
- Spawn Interval: 10 giây
- Auto Spawn: ON
```

---

## Sử Dụng Qua Code (Optional)

### Spawn Shadow Manually
```csharp
ShadowSpawnManager manager = FindObjectOfType<ShadowSpawnManager>();
manager.SpawnRandomShadow(); // Spawn 1 shadow ngẫu nhiên
```

### Start/Stop Spawning
```csharp
manager.StartSpawning(); // Bắt đầu auto spawn
manager.StopSpawning();  // Dừng auto spawn
```

### Destroy All Shadows
```csharp
manager.DestroyAllShadows(); // Xóa tất cả shadows
```

### Check Shadow Count
```csharp
int count = manager.GetActiveShadowCount();
bool canSpawn = manager.CanSpawnMore();
```

---

## Kiểm Tra An Toàn

Hệ thống tự động kiểm tra:
- ✓ Không spawn vào tường/obstacles
- ✓ Không spawn quá gần player
- ✓ Không spawn quá gần victims
- ✓ Giới hạn số lượng shadow tối đa
- ✓ Tự động xóa shadows đã bị destroy khỏi tracking

---

## Troubleshooting

### Shadow không spawn?
1. Kiểm tra Console có errors không
2. Kiểm tra `Spawn Zones` có được assign không
3. Kiểm tra `Shadow Prefabs` có được assign không
4. Kiểm tra `Obstacle Layer` đã được set đúng chưa
5. Kiểm tra đã đạt `Max Shadows In Scene` chưa

### Shadow spawn vào tường?
1. Kiểm tra tường có đúng Layer không
2. Kiểm tra `Obstacle Layer` mask trong ShadowSpawnZone
3. Tăng `Spawn Check Radius`

### Shadow spawn quá gần player?
1. Tăng `Min Distance From Player` trong ShadowSpawnZone

### Spawn quá nhiều/ít?
1. Điều chỉnh `Spawn Interval` (giảm = spawn nhanh hơn)
2. Điều chỉnh `Max Shadows In Scene` (tăng = nhiều shadows hơn)

---

## Tips & Best Practices

1. **Spawn Zones:** Tạo nhiều spawn zones nhỏ hơn là 1 zone lớn
2. **Min Distance:** Set ít nhất 3-5 units để tránh spawn gần player
3. **Max Shadows:** Cân bằng giữa challenge và performance (10-15 là hợp lý)
4. **Weights:** Điều chỉnh để balance difficulty
5. **Test:** Playtest và điều chỉnh parameters theo cảm nhận

---

## Performance Notes

- Hệ thống lightweight, không ảnh hưởng performance
- Cleanup tự động cho destroyed shadows
- Position validation có giới hạn số lần thử (30 attempts)
- Recommended max shadows: 20-30 cho smooth gameplay

