# Truth with Trenchcoat — Unity Scene Layout & Coordinates

All coordinates are **placeholder values**, in meters, using Unity's standard world space
(X = right, Y = up, Z = forward). They are sized generously so the player can walk freely
and view objects at normal VR scale (1 unit = 1 meter, player capsule height ~1.7–1.8m).

Adjust freely once real models exist — what matters is preserving the **topology**
(which room connects to which) from the hand-drawn floor plan, not these exact numbers.

## 1. Room Grid (Top-Down)

```
                 [Storage]     [Server Room]   [Security Room]
                  (-14,0,10)     (0,0,10)         (14,0,10)

 [Entrance]------------------[ MAIN LAB ]------------------[Bathroom]
 (-20,0,0)                    (0,0,0)                      (16,0,0)

                 [Secret Room]   [Dr's Office]
                 (hidden, no       (0,0,-12)
                  direct door,
                  behind office)
```

## 2. Room Data Table

| Room | Center (X,Y,Z) | Footprint (W×D) | Height | Notes |
|---|---|---|---|---|
| Entrance | (-20, 0, 0) | 6m × 6m | 3m | Player spawn point at (-20, 0, -2), facing +Z |
| Main Lab | (0, 0, 0) | 14m × 14m | 3.5m | Central hub — largest room |
| Dr's Office | (0, 0, -12) | 7m × 7m | 3m | Entered from Main Lab (south wall) |
| Secret Room | (0, 0, -22) | 9m × 9m | 3m | NO direct door — only via sliding wall inside Office |
| Bathroom | (16, 0, 0) | 5m × 5m | 3m | Entered from Main Lab (east wall) |
| Security Room | (14, 0, 10) | 7m × 7m | 3m | Entered from Main Lab (north-east) |
| Server Room | (0, 0, 10) | 7m × 7m | 3m | Entered from Main Lab (north) |
| Storage | (-14, 0, 10) | 7m × 7m | 3m | Entered ONLY via Server Room (no direct Main Lab door) |

## 3. Door / Connection Points

| Door ID | Connects | World Position | Locked Until |
|---|---|---|---|
| Door_Entrance_MainLab | Entrance ↔ Main Lab | (-9, 0, 0) | Task 1 (Lockpick found) |
| Door_MainLab_Office | Main Lab ↔ Dr's Office | (0, 0, -6) | Task 1 |
| Door_MainLab_Bathroom | Main Lab ↔ Bathroom | (9, 0, 0) | Task 1 |
| Door_MainLab_SecurityRoom | Main Lab ↔ Security Room | (8, 0, 6.5) | Task 4 (Masterkey found) |
| Door_MainLab_ServerRoom | Main Lab ↔ Server Room | (0, 0, 6.5) | Task 4 |
| Door_ServerRoom_Storage | Server Room ↔ Storage | (-4.5, 0, 10) | Task 4 (opens with Server Room) |
| Wall_Office_SecretRoom | Dr's Office ↔ Secret Room | (0, 0, -16) | Task 8 (sliding wall switch — NOT a normal door, see `SecretWallSwitch.cs`) |

> Door prefabs should use `DoorController.cs` (Section 5). `Wall_Office_SecretRoom` uses
> `SecretWallSwitch.cs` instead — it's a sliding wall trigger, not a hinged door.

## 4. Key Interactive Object Placement (local to their room center, offsets in meters)

| Object | Room | Local Offset from Room Center | World Position (approx) |
|---|---|---|---|
| Lockpick (pickup) | Entrance | (1.5, 0.9, 1) | (-18.5, 0.9, 1) |
| Color Box: Blue | Main Lab | (-3, 1, -2) | (-3, 1, -2) |
| Color Box: Green | Main Lab | (-1, 1, -2) | (-1, 1, -2) |
| Color Box: Yellow | Main Lab | (1, 1, -2) | (1, 1, -2) |
| Color Box: Red | Main Lab | (3, 1, -2) | (3, 1, -2) |
| Sequence Hint Panel | Main Lab | (0, 1.6, -2.3) | (0, 1.6, -2.3) |
| Stopped Watch (desk) | Main Lab | (2, 0.95, 3) | (2, 0.95, 3) |
| Masterkey (pickup, gated) | Main Lab | (2.2, 0.95, 3.1) | (2.2, 0.95, 3.1) — reveal after Task 3 |
| Server Code Panel | Server Room | (-2.5, 1.2, 2.5) | (-2.5, 1.2, 12.5) |
| CCTV Monitor Wall | Security Room | (0, 1.5, 2.8) | (14, 1.5, 12.8) |
| Stuck Drawer | Dr's Office | (-2, 0.7, 1.5) | (-2, 0.7, -10.5) |
| Sliding Wall Painting/Switch | Dr's Office | (0, 1.4, -3.4) | (0, 1.4, -15.4) |
| Wrench (pickup) | Storage | (1, 0.9, -1) | (-13, 0.9, 9) |
| Keychain Piece A | Dr's Office (in drawer) | (-2, 0.7, 1.5) | spawned on drawer open |
| Keychain Piece B | Secret Room | (2, 0.9, -2) | (2, 0.9, -24) |
| Secret Computer / Recorder | Secret Room | (0, 1.0, -3) | (0, 1.0, -25) |
| Final Decision Console | Secret Room | (0, 1.1, 2) | (0, 1.1, -20) |

## 5. Player Rig

- Use the **XR Origin (XR Interaction Toolkit)** prefab as the player rig root.
- Spawn position: `(-20, 0, -2)`, rotation `Y = 0` (facing into Entrance, toward Main Lab).
- Locomotion: continuous move + turn, or teleport — either works with the scripts below;
  none of the puzzle logic depends on locomotion type.
