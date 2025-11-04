# Shadow Spawn System - Troubleshooting Guide

## ❌ Vấn Đề: Shadow Vẫn Spawn Vào Tường

### 🔍 Các Nguyên Nhân & Giải Pháp

---

## 1. Obstacle Layer Chưa Được Set ⚠️

### Triệu Chứng:
- Shadow spawn vào tường thường xuyên
- Console warning: "Obstacle Layer chưa được set"

### Giải Pháp:

**Bước 1: Kiểm tra Layer của tường**
```
1. Select tường trong Scene
2. Inspector → Top → Layer dropdown
3. Phải là: "Ground" hoặc "Wall" hoặc "Obstacle"
4. Nếu là "Default" → ❌ WRONG!
```

**Bước 2: Set Obstacle Layer trong Spawn Zone**
```
1. Select Shadow Spawn Zone
2. Inspector → Obstacle Layer
3. Check các layers: ✓ Ground  ✓ Wall  ✓ Obstacle
4. Save scene
```

---

## 2. Spawn Check Radius Quá Nhỏ ⚠️

### Triệu Chứng:
- Shadow spawn SÁT tường (không trong tường nhưng rất gần)
- Shadow bị stuck khi di chuyển

### Nguyên Nhân:
```
Shadow Collider: ~0.84 units
Spawn Check Radius: 0.5 units (cũ)
→ Buffer chỉ 0.08 units → QUÁ NHỎ!
```

### Giải Pháp:

**Tăng Spawn Check Radius:**
```
1. Select Shadow Spawn Zone
2. Inspector → Spawn Check Radius
3. Đổi từ 0.5 → 1.0 hoặc 1.5
4. Save
```

**Giá Trị Đề Xuất:**
- `1.0` = Tốt (buffer 0.58 units)
- `1.5` = Rất tốt (buffer 1.08 units) ✅ KHUYÊN DÙNG
- `2.0` = Siêu an toàn (nhưng khó tìm vị trí)

---

## 3. Tường Không Có Collider ❌

### Triệu Chứng:
- Shadow spawn XUYÊN QUA tường
- Tường không block player

### Giải Pháp:

**Kiểm tra Collider:**
```
1. Select tường
2. Inspector → Phải có component:
   - BoxCollider2D HOẶC
   - TilemapCollider2D HOẶC
   - PolygonCollider2D
3. Nếu không có → Add Component → Box Collider 2D
```

---

## 4. Zone Position Nằm Trong Tường 🏗️

### Triệu Chứng:
- Console: "Không tìm được vị trí hợp lệ sau 50 lần thử"
- Shadow không spawn hoặc spawn ở center (trong tường)

### Giải Pháp:

**Di chuyển Zone:**
```
1. Select Shadow Spawn Zone trong Hierarchy
2. Scene view → Di chuyển đến VỊ TRÍ TRỐNG (không có tường)
3. Trong Scene, bạn sẽ thấy purple box (spawn zone)
4. Đảm bảo purple box KHÔNG đè lên tường
```

**Zone Size:**
- Nếu zone quá lớn và bị tường cắt → Giảm Zone Size
- Hoặc tạo nhiều zones nhỏ hơn

---

## 5. Double Check Safety Bị Fail 🛡️

### Hệ Thống Mới (v2):

Code bây giờ có **DOUBLE CHECK** trước khi spawn:

```csharp
// Check 1: Zone tìm vị trí (50 attempts)
Vector2 position = zone.GetRandomValidPosition();

// Check 2: Verify position != Vector2.zero
if (position == Vector2.zero) → SKIP SPAWN

// Check 3: Double check collision trước khi Instantiate
if (có obstacle tại position) → CANCEL SPAWN
```

### Nếu Vẫn Spawn Vào Tường:

**Điều này CỰC KỲ HIẾM XẢY RA** nếu setup đúng!

Check:
1. ✅ Tường có Layer đúng?
2. ✅ Tường có Collider?
3. ✅ Obstacle Layer mask correct?
4. ✅ Spawn Check Radius ≥ 1.0?

---

## 🧪 Testing Tools

### Tool 1: Validate Zone Setup (QUAN TRỌNG!)

```
1. Select Shadow Spawn Zone
2. Inspector → Click button "✓ Validate Zone Setup"
3. Xem kết quả:
   - ✅ Green = OK
   - ⚠️ Yellow = Warning
   - ❌ Red = Error
4. Fix theo hướng dẫn
```

### Tool 2: Validate All Zones

```
1. Select Shadow Spawn Manager
2. Inspector → Click "Validate All Spawn Zones"
3. Check Console logs
4. Fix các zones bị lỗi
```

### Tool 3: Test Get Valid Position

```
1. Select Shadow Spawn Zone
2. Inspector → Click "Test Get Valid Position"
3. Check Console → Vị trí có hợp lệ?
4. Check Scene view → Vị trí có nằm trong tường không?
```

### Tool 4: Preview Multiple Positions

```
1. Select Shadow Spawn Zone
2. Inspector → Click "Preview 10 Random Positions"
3. Check Console:
   - Tìm được 7-10/10 → ✅ Tốt
   - Tìm được 3-6/10 → ⚠️ Zone hơi chật
   - Tìm được 0-2/10 → ❌ Zone quá chật hoặc setup sai
```

---

## 🔧 Quick Fix Checklist

Nếu shadow vẫn spawn vào tường, làm theo thứ tự:

- [ ] **Step 1:** Click "Validate All Spawn Zones" button
- [ ] **Step 2:** Fix các zones bị lỗi
- [ ] **Step 3:** Verify tường có Layer + Collider
- [ ] **Step 4:** Set Obstacle Layer mask trong zones
- [ ] **Step 5:** Tăng Spawn Check Radius lên 1.5
- [ ] **Step 6:** Di chuyển zones ra khỏi tường
- [ ] **Step 7:** Test trong Play mode
- [ ] **Step 8:** Check Console logs

---

## 📊 Debug Logs Guide

### Logs Bình Thường (OK):

```
✓ ShadowSpawnZone 'Zone 1': Tìm được vị trí hợp lệ sau 3 lần thử - Position: (10.5, 8.2)
✓ ShadowSpawnManager: Spawned Shadow Ghost (Spawned) tại (10.5, 8.2)
```

### Logs Cảnh Báo (Warning):

```
⚠️ ShadowSpawnManager: Zone 'Zone 1' không tìm được vị trí hợp lệ. SKIP spawn lần này.
```
→ Zone có thể quá chật. Di chuyển hoặc tăng size.

### Logs Lỗi (Error):

```
❌ ShadowSpawnZone 'Zone 1': KHÔNG tìm được vị trí hợp lệ sau 50 lần thử!
```
→ Zone bị block hoàn toàn. Check setup ngay!

```
⚠️ ShadowSpawnManager: VỊ TRÍ KHÔNG AN TOÀN! Có obstacle tại (x, y). CANCEL spawn.
```
→ Double check phát hiện vấn đề. Shadow KHÔNG được spawn (an toàn).

---

## 🎯 Best Practices

### 1. Zone Placement

✅ **TỐT:**
```
- Zones ở giữa phòng trống
- Zones không overlap với tường
- Zones có ít nhất 50% không gian trống
```

❌ **TỆ:**
```
- Zones nằm trong tường
- Zones quá chật, toàn obstacles
- Zones quá nhỏ (< 5x5)
```

### 2. Multiple Zones

Tốt hơn:
- 5 zones nhỏ (8x8) ở 5 phòng khác nhau
  
Hơn là:
- 1 zone lớn (40x40) cover toàn map

### 3. Spawn Check Radius

Luôn dùng:
- `1.5` cho zones bình thường
- `2.0` cho zones rất chật
- KHÔNG BAO GIỜ dùng `< 1.0`

### 4. Regular Validation

Mỗi khi:
- Thay đổi map layout
- Thêm/xóa tường
- Di chuyển zones

→ Click "Validate All Spawn Zones"!

---

## 📞 Vẫn Không Work?

### Checklist Cuối Cùng:

1. **Unity Version**: Đảm bảo dùng Unity 6
2. **Physics 2D Settings**: 
   - Edit → Project Settings → Physics 2D
   - Verify layers có collision đúng
3. **Layer Collision Matrix**:
   - Enemy layer có collide với Ground/Wall không?
4. **Tilemap Setup**:
   - Nếu dùng Tilemap → Phải có TilemapCollider2D
5. **Composite Collider**:
   - Nếu dùng Composite Collider → Verify setup

---

## 🆘 Emergency Fix

Nếu HOÀN TOÀN không work:

### Option 1: Reset Zones
```
1. Delete tất cả Shadow Spawn Zones
2. Menu → GameObject → Shadow Spawn → Setup Complete System
3. Di chuyển zones đến vị trí đúng
4. Validate lại
```

### Option 2: Manual Override
```
1. Trong ShadowSpawnZone.cs
2. Tìm line: int maxAttempts = 50;
3. Đổi thành: int maxAttempts = 100;
4. Tìm line: spawnCheckRadius = 1.0f;
5. Đổi thành: spawnCheckRadius = 2.0f;
6. Save và test
```

---

## ✅ Kiểm Tra Thành Công

Bạn biết hệ thống work khi:

- ✅ Shadow spawn ở giữa phòng (không gần tường)
- ✅ Console không có errors
- ✅ Validate All Zones → 100% valid
- ✅ Play 5-10 phút không thấy shadow trong tường
- ✅ Logs: "Tìm được vị trí hợp lệ sau X lần thử" (X < 10)

---

## 📚 Related Files

- `QUICK_START_VI.md` - Setup guide
- `SHADOW_SPAWN_SETUP_GUIDE.md` - Chi tiết setup
- `SHADOW_SPAWN_FEATURES.md` - Kỹ thuật
- `SHADOW_SPAWN_SUMMARY.txt` - Tóm tắt

---

**Good luck! 🎮**

