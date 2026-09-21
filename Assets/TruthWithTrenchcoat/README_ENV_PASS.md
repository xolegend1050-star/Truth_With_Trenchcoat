# Truth With Trenchcoat — Detailed Environment Pass

## What this package does
This is **Environment Pass 1**. It builds a complete, walkable-looking facility shell and a much more dressed procedural prototype directly in Unity using primitives. It follows the room connectivity and lighting rules in the supplied GDD and uses the supplied Unity.pdf reference board for visual direction and prop families.

### Rooms generated
- Entrance
- Main Lab
- Bathroom
- Dr.'s Office
- Secret Room (hidden-room location)
- Server Room
- Storage
- Security Room

### Dressing generated
Main lab: work table, chairs, monitors, laptop, cabinets, shelves, stopped watch, broken phone, clue mark.
Office: desk, chair, monitor, lamp, cabinet, shelf, stuck drawer, wall painting/marking.
Storage: shelving/cabinets, boxes, wrench, keychain half.
Server: server racks, units, LEDs, power restoration panel.
Security: CCTV monitor wall, console desk, chair.
Bathroom: sink, mirror, toilet, stall partitions.
Secret room: computer/workstation, ECHO display, recorder, documents, second keychain half.
Entrance: reception/entry desk, chair, lockpick.

## Install
1. Create/open a Unity 3D project.
2. Copy the `Assets/TruthWithTrenchcoat` folder into your project's `Assets` folder.
3. Open any scene.
4. Use **Truth With Trenchcoat > Build Detailed Environment**.
5. Save the scene.

## Important
This is not a final photorealistic asset library. It is a procedural, detailed environment pass designed to make the game architecture and visual dressing concrete before replacing selected primitives with final meshes/materials.

The GDD says the hand-drawn floor plan is the authority for room structure, while the reference infographics are visual/object-placement references. The infographic's puzzle text is intentionally not used as gameplay logic.

## Next art pass
Replace the reusable primitive props with authored/imported meshes for: lab benches, chairs, monitors, server racks, office furniture, bathroom fixtures, doors, cabinets, lab equipment, and decorative clutter. Keep the generated room hierarchy and named gameplay objects so the gameplay scripts can be wired to them.
