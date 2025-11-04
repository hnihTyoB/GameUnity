# Shadow Spawn System - Tính Năng & Chi Tiết Kỹ Thuật

## Tổng Quan Hệ Thống

Hệ thống spawn shadow ngẫu nhiên **AN TOÀN** với các cơ chế kiểm tra và bảo vệ để tránh lỗi khi spawn vào vị trí không hợp lệ.

---

## Files Đã Tạo

### 1. Core Scripts (Assets/Scripts/Enemies/)
- **ShadowSpawnZone.cs** - Component định nghĩa vùng spawn
- **ShadowSpawnManager.cs** - Manager điều khiển spawning logic

### 2. Editor Scripts (Assets/Scripts/Editor/)
- **ShadowSpawnZoneEditor.cs** - Custom inspector cho Spawn Zones
- **ShadowSpawnManagerEditor.cs** - Custom inspector cho Spawn Manager  
- **ShadowSpawnMenuItems.cs** - Menu shortcuts để tạo objects nhanh

### 3. Documentation
- **SHADOW_SPAWN_SETUP_GUIDE.md** - Hướng dẫn setup chi tiết
- **SHADOW_SPAWN_FEATURES.md** - File này (tính năng & kỹ thuật)

---

## Tính Năng Chính

### ✅ An Toàn Khi Spawn

1. **Collision Detection**
   - Kiểm tra va chạm với tường/obstacles trước khi spawn
   - Sử dụng `Physics2D.OverlapCircle` với configurable radius
   - Tránh spawn vào vị trí blocked

2. **Distance Validation**
   - Không spawn quá gần player (configurable min distance)
   - Không spawn quá gần victims
   - Đảm bảo gameplay fair

3. **Retry Logic**
   - Tự động thử lại 30 lần để tìm vị trí hợp lệ
   - Fallback về center zone nếu không tìm được
   - Không bao giờ crash/freeze

4. **Layer Mask System**
   - Flexible obstacle detection qua Unity Layer Masks
   - Có thể chọn layers nào cần tránh (Ground, Wall, Obstacle, etc.)

### ✅ Quản Lý Thông Minh

1. **Max Limit Control**
   - Giới hạn số shadow tối đa trong scene
   - Tự động tracking shadows đang active
   - Cleanup shadows đã bị destroy

2. **Weighted Random Selection**
   - Spawn shadows theo xác suất (weights)
   - VD: Shadow Ghost 1 (60%), Ghost 2 (30%), Ghost 3 (10%)
   - Dễ dàng balance difficulty

3. **Auto Spawn System**
   - Tự động spawn theo intervals
   - Configurable spawn rate
   - Initial delay trước khi spawn lần đầu

4. **Manual Control**
   - Có thể spawn manually qua code
   - Start/Stop spawning bất cứ lúc nào
   - Destroy all shadows on demand

### ✅ Editor Integration

1. **Visual Gizmos**
   - Vẽ spawn zones màu tím trong Scene view
   - Hiển thị min distance từ player (vòng tròn vàng)
   - Preview spawn positions (vòng tròn xanh)

2. **Inspector Buttons**
   - Test spawn positions trong Editor
   - Preview multiple random positions
   - Runtime controls (spawn now, start/stop, destroy all)

3. **Quick Setup Menu**
   - `GameObject → Shadow Spawn → Create Spawn Zone`
   - `GameObject → Shadow Spawn → Create Spawn Manager`
   - `GameObject → Shadow Spawn → Setup Complete System`

---

## Chi Tiết Kỹ Thuật

### ShadowSpawnZone Component

**Properties:**
```csharp
- Vector2 zoneSize          // Kích thước vùng spawn
- LayerMask obstacleLayer   // Layers cần tránh
- float minDistanceFromPlayer // Khoảng cách min từ player
- float spawnCheckRadius    // Radius kiểm tra collision
```

**Methods:**
```csharp
public Vector2 GetRandomValidPosition()
- Trả về vị trí hợp lệ ngẫu nhiên trong zone
- Tự động kiểm tra collision, distance, etc.
- Retry logic với max attempts

private bool IsPositionValid(Vector2 position)
- Kiểm tra: obstacles, player distance, victim distance
- Return true nếu vị trí OK để spawn
```

**Gizmos:**
- Purple cube: Vùng spawn
- Yellow circle (selected): Min distance from player
- Green circles (selected): Sample spawn positions

---

### ShadowSpawnManager Component

**Properties:**
```csharp
- GameObject shadowGhost1Prefab
- GameObject shadowGhost2Prefab  
- GameObject shadowGhost3Prefab
- int maxShadowsInScene         // Giới hạn
- float spawnInterval           // Thời gian giữa spawns
- float initialSpawnDelay       // Delay ban đầu
- bool autoSpawn                // Auto spawn on/off
- int shadowGhost1Weight        // Xác suất (0-100)
- int shadowGhost2Weight
- int shadowGhost3Weight
- ShadowSpawnZone[] spawnZones  // Zones để spawn
```

**Public Methods:**
```csharp
void StartSpawning()
- Bắt đầu auto spawn coroutine

void StopSpawning()
- Dừng auto spawn

GameObject SpawnRandomShadow()
- Spawn 1 shadow ngẫu nhiên tại zone ngẫu nhiên
- Return GameObject spawned (hoặc null nếu fail)

GameObject SpawnShadowAtPosition(GameObject prefab, Vector2 pos)
- Spawn shadow cụ thể tại vị trí cụ thể
- Manual control

void DestroyAllShadows()
- Xóa tất cả shadows đang active

int GetActiveShadowCount()
- Return số shadows hiện tại

bool CanSpawnMore()
- Check có thể spawn thêm không
```

**Private Methods:**
```csharp
IEnumerator AutoSpawnRoutine()
- Coroutine chạy auto spawn
- Loop: delay → cleanup → spawn → repeat

GameObject GetRandomShadowPrefab()
- Random selection dựa trên weights
- Weighted probability algorithm

void CleanupDestroyedShadows()
- Remove null references từ tracking list
- Gọi tự động trong các methods
```

---

## Workflow Khi Spawn

```
1. Manager triggers spawn (auto hoặc manual)
   ↓
2. Check: activeShadows.Count < maxShadowsInScene?
   ↓ (NO → Cancel)
   ↓ (YES)
3. Select random SpawnZone
   ↓
4. Zone.GetRandomValidPosition()
   ↓
5. Loop (max 30 attempts):
   - Generate random position trong zone
   - IsPositionValid()?
     - Check collision với obstacles
     - Check distance từ player
     - Check distance từ victims
   ↓ (Invalid → retry)
   ↓ (Valid)
6. Select random shadow prefab (weighted)
   ↓
7. Instantiate shadow tại valid position
   ↓
8. Add vào activeShadows list
   ↓
9. Return spawned GameObject
```

---

## Performance Considerations

### Optimized
- ✓ Không có `FindObjectsOfType` trong loop (chỉ 1 lần cho victims check)
- ✓ Cleanup automatic cho destroyed shadows
- ✓ Max attempts limit (30) tránh infinite loops
- ✓ Lightweight collision checks (OverlapCircle)

### Best Practices
- Giới hạn max shadows: 10-20 cho optimal performance
- Spawn interval: Ít nhất 5-10 giây
- Spawn zones: 3-5 zones per scene
- Position validation: Keep obstacleLayer mask minimal

---

## Extension Ideas (Future)

1. **Wave System**
   - Spawn nhiều shadows theo waves
   - Tăng difficulty theo thời gian

2. **Conditional Spawning**
   - Spawn khi player vào trigger zone
   - Spawn based on game events

3. **Spawn Pools**
   - Object pooling thay vì Instantiate/Destroy
   - Better performance

4. **Spawn Patterns**
   - Circular spawn around player
   - Path-based spawning

5. **Statistics Tracking**
   - Track số shadows spawned
   - Track kill rate
   - Analytics cho balancing

---

## Integration với Hệ Thống Hiện Tại

### Compatible với:
- ✓ EnemyAI.cs (shadows sử dụng AI system hiện tại)
- ✓ ShadowGhost.cs, ShadowGhost2.cs, ShadowGhost3.cs
- ✓ LightFearBehavior.cs (shadows vẫn sợ ánh sáng)
- ✓ FlashlightEffect.cs (flashlight vẫn kill shadows)
- ✓ PickUpSpawner.cs (shadows drop batteries khi chết)

### Không ảnh hưởng:
- Player systems
- Victim systems  
- Other enemy types (Slime, Grape, etc.)
- UI/Camera/Management systems

---

## Troubleshooting Common Issues

### Issue: Shadows spawn vào tường hoặc sát tường quá
**Nguyên nhân:**
- Spawn Check Radius quá nhỏ (< 1.0)
- Shadow collider (~0.84 units) lớn hơn check radius
- Tường chưa có Layer hoặc Collider

**Solution:**
- **Tăng Spawn Check Radius lên 1.0 hoặc 1.5** (quan trọng nhất!)
- Kiểm tra Obstacle Layer mask trong ShadowSpawnZone
- Đảm bảo tường có đúng layer
- Verify tường có Collider2D component

### Issue: Không spawn được shadow nào
**Solution:**
- Check Console cho errors/warnings
- Verify shadow prefabs được assign
- Verify spawn zones tồn tại và active
- Check đã đạt maxShadowsInScene chưa
- Test GetRandomValidPosition() trong Inspector

### Issue: Spawn quá nhiều/ít
**Solution:**
- Adjust spawnInterval (giảm = spawn nhanh hơn)
- Adjust maxShadowsInScene
- Check autoSpawn có được bật không

### Issue: Performance drops
**Solution:**
- Giảm maxShadowsInScene
- Tăng spawnInterval
- Reduce số spawn zones
- Optimize obstacle layer (chỉ include necessary layers)

---

## Testing Checklist

- [ ] Shadows spawn tại valid positions (không vào tường)
- [ ] Không spawn quá gần player
- [ ] Respects max shadow limit
- [ ] Weighted random works correctly
- [ ] Auto spawn starts/stops correctly
- [ ] Manual spawn works
- [ ] Destroy all shadows works
- [ ] Gizmos hiển thị đúng trong Scene view
- [ ] Inspector buttons work in Play mode
- [ ] No errors/warnings trong Console
- [ ] Performance acceptable (60 FPS với max shadows)

---

## Version History

**v1.0** (04/11/2025)
- Initial implementation
- Core spawning system
- Safety checks & validation
- Editor tools & gizmos
- Documentation

---

## Credits

Developed for: **2D Top Down RPG Project**  
Unity Version: **Unity 6**  
Created: **04/11/2025**

