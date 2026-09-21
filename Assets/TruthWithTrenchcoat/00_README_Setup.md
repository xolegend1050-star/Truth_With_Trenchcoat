# Truth with Trenchcoat — Unity Implementation Package

This package covers everything needed to start building the actual Unity project:
scene coordinates for blockout, and every core/puzzle/UI script referenced in the
Game Design Document, wired for **Unity XR Interaction Toolkit**.

## Package Contents

```
01_SceneLayout_Coordinates.md      <- room positions, door positions, prop placement
Scripts/
  Core/
    GameManager.cs                 <- central task state machine (Task 1–10)
    ClueJournal.cs                 <- tracks the 3 clues
    PlayerInventory.cs             <- lightweight "has item" tracking
  Interaction/
    CollectibleItem.cs             <- base pickup script (lockpick, masterkey, wrench, keychain pieces)
    DoorController.cs              <- auto-unlock/open doors based on game state
  Puzzles/
    ColorSequencePuzzle.cs         <- Task 2: restore the lights
    ServerCodePanel.cs             <- Task 5: restore the server (code 1423)
    CCTVFootagePuzzle.cs           <- Task 6: Clue 1, missing footage
    DrawerController.cs            <- Task 3/7: stuck drawer, needs wrench
    SecretWallSwitch.cs            <- Task 8: painting/switch opens secret room
    SecretComputer.cs              <- Task 9: Clue 3, Project ECHO reveal
    FinalDecisionManager.cs        <- Task 10: 3 endings
  UI/
    InstructionUI.cs               <- on-screen task instruction text
    ClueJournalUI.cs               <- optional "clue collected" popup
```

## 1. Unity Project Setup

1. Create a new Unity project using the **3D (URP)** template (VR-friendly rendering).
2. Install via Package Manager:
   - `XR Interaction Toolkit`
   - `XR Plug-in Management` (enable the runtime matching your headset — OpenXR is the
     safest general-purpose choice since you're not locked into one headset yet)
   - `TextMeshPro` (for the instruction UI / clue popups)
3. Import the **XR Interaction Toolkit → Starter Assets** sample from the Package
   Manager. This gives you a ready-made `XR Origin (XR Rig)` prefab with hand
   presence and grab/poke interactors already configured — use this as your player
   rig rather than building one from scratch.

## 2. Blockout the Level

1. Follow `01_SceneLayout_Coordinates.md` — create empty GameObjects (or simple
   cubes/ProBuilder blocks) at each room's center position, sized to the listed
   footprint, to rough out the facility. This gives you a walkable, correctly
   connected space immediately, before any real art exists.
2. Place door GameObjects at each listed door position. Each gets a
   `DoorController` component (see Section 4 below).
3. Place the interactive props from the "Key Interactive Object Placement" table.

## 3. Add the Core Managers

Create one empty GameObject in your scene called `_GameManagers`, and add:
- `GameManager.cs`
- `ClueJournal.cs`
- `PlayerInventory.cs`

These are singletons (`Instance` static references) — only ever have one of each
in the scene. They persist for the whole play session; no `DontDestroyOnLoad` is
needed unless you're using multiple Unity scenes (this game is designed as a
single continuous scene, per the linear task flow).

## 4. Wire Up Doors

For every door except the Office↔Secret Room wall:
1. Add an `XR Grab Interactable` OR just a plain `Collider` (doors don't need to
   be grabbed — they open automatically) — a trigger collider is enough if you
   want proximity-based opening, but per the design doc, doors open the instant
   their unlock condition is met, not on player proximity. So: no interactor
   needed at all — just add `DoorController.cs` directly.
2. Set `condition` in the Inspector to match the table in the coordinates doc
   (RequiresLockpick / RequiresMasterkey / RequiresServerPower / None).
3. Either assign an `Animator` with an `IsOpen` bool parameter, OR assign
   `doorTransform` + `openLocalPositionOffset` for a simple slide animation
   (no animator needed for a placeholder blockout).

For the Office↔Secret Room wall: use `SecretWallSwitch.cs` instead (see below).

## 5. Wire Up Pickup Items

For the Lockpick, Masterkey, Wrench, and both Keychain pieces:
1. Add an `XR Grab Interactable` component (from XRI) to each prop.
2. Add `CollectibleItem.cs` alongside it.
3. Set `itemId` to exactly one of: `Lockpick`, `Masterkey`, `Wrench`,
   `KeychainPieceA`, `KeychainPieceB` (case-sensitive — other scripts match on
   these exact strings).
4. Leave `keepAsHeldObject = true` for all of them (the wrench in particular
   must stay equippable, since `DrawerController` checks for it being held).

> Note: the Masterkey should be placed inactive/hidden in the scene initially
> and only `SetActive(true)`'d once Task 3 completes — wire this via a
> `GameManager.OnTaskChanged` listener on a small helper script, or simply
> place it physically behind/inside whatever Task 3 reveals.

## 6. Wire Up Each Puzzle

| Puzzle | Component | Key fields to assign |
|---|---|---|
| Lights (Task 2) | `ColorSequencePuzzle` | 4 `XR Simple Interactable` boxes in the `boxes` list, each tagged with its `BoxColor` |
| Server (Task 5) | `ServerCodePanel` | Wire your keypad's 0–9 buttons to call `SubmitDigit(int)` |
| CCTV (Task 6) | `CCTVFootagePuzzle` | Add `XR Simple Interactable` to the monitor wall prop |
| Drawer (Task 3/7) | `DrawerController` | Add `XR Simple Interactable`; assign `keychainPieceAObject` |
| Secret Wall (Task 8) | `SecretWallSwitch` | Add `XR Simple Interactable` to the painting prop |
| Secret Computer (Task 9) | `SecretComputer` | Add `XR Simple Interactable`; assign `arjunRecordingAudio` clip |
| Final Decision (Task 10) | `FinalDecisionManager` | Wire 3 buttons/interactables to `ChooseReport()`, `ChooseProtect()`, `ChooseExpose()` |

`XR Simple Interactable` (not Grab) is correct for all "press/interact" props
(buttons, drawers, paintings, computers) — Grab is only for things the player
physically picks up and carries (Section 5 above).

## 7. Wire Up UI

1. Create a world-space Canvas (recommended: parented to the player's wrist or
   a fixed HUD anchor) with a `TMP_Text` for instructions.
2. Add `InstructionUI.cs` to the Canvas, assign the `TMP_Text` reference.
3. It updates automatically — no further wiring needed.
4. (Optional) Repeat for `ClueJournalUI.cs` with a popup panel + 2 `TMP_Text`
   fields (title/body).

## 8. Playtest Order Sanity Check

Because `GameManager.AdvanceTo()` refuses to move backwards or re-fire, you can
playtest any individual puzzle in isolation by manually setting `currentTask`
in the Inspector (while in Play Mode) to jump straight to it — useful for
testing Task 8 or 9 without replaying the whole lockpick→lights→server chain
every time.

## 9. Known Placeholder Assumptions (adjust freely)

- All room dimensions and prop positions are placeholders sized for comfortable
  VR roaming — not derived from real assets.
- The server code (`1423`) and light sequence (Blue→Green→Yellow→Red) are fixed
  per the design doc — do not randomize them unless you intentionally want
  per-playthrough variation (the original notes floated randomizing the CCTV
  puzzle specifically, not these two).
- Ending sequences (`reportEndingSequence` etc.) are left as empty GameObject
  slots for you to fill with your own cutscene/UI content — this package only
  wires the branching logic, not the ending presentation itself.
