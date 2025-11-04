# 🔥 FIX: Shadow Vẫn Spawn Vào Tường

## ⚡ Quick Fix (Làm Ngay!)

### Bước 1: Add Debug Component

```
1. Select "Shadow Spawn Manager" trong Hierarchy
2. Inspector → Add Component → "Shadow Spawn Debugger"
3. Check "Enable Debug Visualization" ✓
```

### Bước 2: Play & Observe

```
1. Play game
2. Mở Scene view (tab Scene bên cạnh Game)
3. Watch màu sắc khi shadow spawn:
   
   🟢 GREEN CIRCLE = Vị trí được chấp nhận
   🔴 RED X = Vị trí bị reject (có obstacle)
   🔵 CYAN CIRCLE = Shadow đã spawn thành công
```

### Bước 3: Check Console

Khi thấy error:

```
⚠️ VỊ TRÍ KHÔNG AN TOÀN! Phát hiện 'Tường_01' (Layer: Default)
🔍 CHECK NGAY: Tường 'Tường_01' có Layer 'Default' có nằm trong Obstacle Layer mask không?
```

→ **VẤN ĐỀ TÌM THẤY!**

---

## 🎯 Root Cause Analysis

### Nguyên Nhân #1: Layer Không Khớp ❌

**Vấn đề:**
```
Tường của bạn:        Layer = "Default" hoặc "Walls"
Obstacle Layer mask:  Chỉ check "Ground", "Wall", "Obstacle"
→ KHÔNG KHỚP → Không detect được!
```

**Giải Pháp:**

**Option A: Đổi Layer của tường (Khuyên dùng)**
```
1. Select TẤT CẢ tường trong scene (Ctrl+Click)
2. Inspector → Layer → Chọn "Wall" (hoặc tạo mới nếu chưa có)
3. Apply to all children if prompted
```

**Option B: Update Obstacle Layer mask**
```
1. Select Shadow Spawn Zone
2. Inspector → Obstacle Layer
3. Check thêm layer của tường (VD: "Default" nếu tường đang dùng Default)
4. Lưu ý: Không nên dùng Default layer cho tường!
```

### Nguyên Nhân #2: Tường Không Có Collider ❌

**Check:**
```
1. Select tường
2. Inspector → Phải có:
   - BoxCollider2D HOẶC
   - TilemapCollider2D HOẶC
   - PolygonCollider2D
```

**Fix:**
```
1. Select tường không có collider
2. Add Component → Box Collider 2D
3. Adjust size to match sprite
```

### Nguyên Nhân #3: Collider Bị Disabled ❌

**Check:**
```
1. Select tường
2. Inspector → Collider component
3. Checkbox bên trái component name phải ✓ (enabled)
```

---

## 🧪 Diagnostic Tool

### Test 1: Layer Check

```csharp
// Paste vào Console (menu: Window → General → Console → Clear → Input)

// Test xem tường có layer gì:
GameObject wall = GameObject.Find("Tên_Tường_Của_Bạn");
Debug.Log($"Wall Layer: {LayerMask.LayerToName(wall.layer)}");

// Test xem zone check layers nào:
ShadowSpawnZone zone = FindObjectOfType<ShadowSpawnZone>();
Debug.Log($"Zone checks layers: {zone.GetObstacleLayer().value}");
```

### Test 2: Collision Check Manual

```
1. Select Shadow Spawn Zone
2. Inspector → Click "✓ Validate Zone Setup"
3. Xem kết quả:
   - "Setup OK! Tìm được X/10 vị trí hợp lệ" → ✅
   - "KHÔNG tìm được vị trí hợp lệ nào!" → ❌ Fix ngay
```

### Test 3: Visual Debug

```
1. Play game với ShadowSpawnDebugger attached
2. Watch Scene view
3. Khi shadow spawn:
   - Thấy GREEN → OK
   - Thấy RED X → Check Console log
   - Không thấy gì → Debugger chưa enable
```

---

## 📋 Complete Checklist

Làm theo thứ tự:

- [ ] **1. Add ShadowSpawnDebugger** vào Shadow Spawn Manager
- [ ] **2. Enable Debug Visualization** trong Debugger
- [ ] **3. Play game** và watch Scene view
- [ ] **4. Check Console** khi có error
- [ ] **5. Verify tường có Layer** đúng (KHÔNG phải Default)
- [ ] **6. Verify tường có Collider** và đang enabled
- [ ] **7. Verify Obstacle Layer mask** bao gồm layer của tường
- [ ] **8. Validate All Spawn Zones** (button trong Manager)
- [ ] **9. Tăng Spawn Check Radius** lên 1.5 nếu cần
- [ ] **10. Test lại** trong Play mode

---

## 🔬 Advanced Debug

### Enable Verbose Logs

Trong `ShadowSpawnZone.cs`, tìm method `IsPositionValid()`:

```csharp
private bool IsPositionValid(Vector2 position)
{
    // Thêm log này vào đầu method:
    Debug.Log($"[CHECK] Testing position: {position}");
    
    // 1. Kiểm tra va chạm với tường/obstacles
    Collider2D hitCollider = Physics2D.OverlapCircle(position, spawnCheckRadius, obstacleLayer);
    if (hitCollider != null)
    {
        // Thêm log này:
        Debug.LogWarning($"[REJECT] Hit obstacle: {hitCollider.name} at {position}");
        return false;
    }
    
    // ... rest of code
}
```

→ Bạn sẽ thấy CHÍNH XÁC vị trí nào bị reject và tại sao!

### Layer Mask Decoder

Để decode LayerMask value:

```
LayerMask value = 256

Binary:   100000000
Layer:    8 (bit thứ 8 = 1)
Name:     "Ground" (nếu layer 8 = Ground)

→ Mask này CHỈ check layer 8!
```

---

## 💡 Prevention Tips

### Setup Tường Đúng Cách

```
1. Tạo layer "Wall" (hoặc "Walls")
2. Set TẤT CẢ tường = layer này
3. Add Collider cho tường
4. Set Obstacle Layer mask trong zones
5. Validate setup trước khi play
```

### Prefab Template

Tạo prefab "Wall_Template":
```
GameObject: Wall
├─ Layer: Wall
├─ Tag: Untagged
└─ Components:
   ├─ Sprite Renderer
   └─ Box Collider 2D ✓
```

Duplicate prefab này cho mọi tường mới!

---

## ❓ FAQ

### Q: Console log nói layer là "Default", sao vậy?

**A:** Tường chưa được set layer! Default là layer 0 (mặc định). Phải set thành "Wall" hoặc layer khác.

### Q: Tôi đã set layer rồi mà vẫn spawn vào tường?

**A:** Check:
1. Tường có Collider không?
2. Collider có enabled không?
3. Obstacle Layer mask có include layer đó không?
4. Scene đã được Save chưa? (Ctrl+S)

### Q: Debug lines không hiện trong Scene view?

**A:** 
1. Scene view phải được mở
2. Scene view phải ở 2D mode
3. Gizmos button (top right Scene view) phải ON
4. ShadowSpawnDebugger phải attached và enabled

### Q: Làm sao biết layer nào là layer nào?

**A:** 
```
Edit → Project Settings → Tags and Layers
Xem list layers từ 0-31
```

---

## 🆘 Still Not Working?

### Last Resort Fixes

**Fix 1: Force Layer Setup**
```
1. Create new layer "ShadowWall"
2. Set ALL walls to this layer
3. Set ONLY this layer in Obstacle Layer mask
4. Save scene
5. Test
```

**Fix 2: Increase Safety Margin**
```
1. Select all Spawn Zones
2. Spawn Check Radius → 2.0 (very safe)
3. Min Distance From Player → 8.0
4. Test
```

**Fix 3: Recreate Zones**
```
1. Delete all Shadow Spawn Zones
2. GameObject → Shadow Spawn → Create Spawn Zone
3. Di chuyển đến vị trí GIỮA PHÒNG (xa tường)
4. Zone Size → 5x5 (nhỏ hơn)
5. Setup layers
6. Test
```

---

## ✅ Success Criteria

Bạn biết đã fix khi:

1. ✅ Console: "✓ ShadowSpawnZone 'Zone X': Tìm được vị trí hợp lệ sau X lần thử"
2. ✅ Scene view: Thấy GREEN circles, CYAN circles (không có RED X)
3. ✅ Game view: Shadow spawn giữa phòng, không gần tường
4. ✅ Play 10 phút: Không thấy shadow trong/sát tường
5. ✅ Validate: "✓ Valid Zones: X, ❌ Invalid Zones: 0"

---

## 📞 Debug Report Template

Nếu vẫn không work, post thông tin này:

```
=== SHADOW SPAWN DEBUG REPORT ===

1. Unity Version: [Unity 6 / Unity 2022.x / etc]

2. Tường Setup:
   - Layer: [tên layer]
   - Collider: [BoxCollider2D / TilemapCollider2D / None]
   - Collider Enabled: [Yes / No]

3. Spawn Zone Setup:
   - Obstacle Layer mask: [layers được check]
   - Spawn Check Radius: [value]
   - Zone Size: [value]

4. Console Error:
   [copy paste error log]

5. Screenshot:
   [Scene view với debug visualization]

6. Validate Result:
   [copy paste validation log]
```

---

**Good luck! Với debug tools mới, bạn sẽ TÌM ĐƯỢC vấn đề chính xác! 🔍**

