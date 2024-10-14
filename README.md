
# MH-6 Little Bird Flight Simulator

A fun and engaging flight simulator focused on the MH-6 Little Bird helicopter, providing a variety of game modes and accurate collision detection.

## Features

**Collision Detection**:
- Accurate collision detection with the ground and obstacles.

**Point System**:
- Earn points based on close contacts and precision.

**Game Modes**:
- *Time Attack*: Race against the clock to reach waypoints while hitting targets with the payloads provided.
- *Free Flight*: Explore the environment at your own pace without restrictions.
- *Obstacle Course*: Navigate through challenging courses designed to test your maneuvering skills.

**Bluetooth/Serial Support for ESP32**:
- Integration with an ESP32 microcontroller for additional hardware features.
- The game sends out serial/Bluetooth messages to the ESP32 every **0.25 seconds** in the following format:
  - `YAW-float`
  - `PITCH-float`
  - `ROLL-float`
  - `SURGE-float` (Forward/Backward movement)
  - `SWAY-float` (Left/Right movement)
  - `HEAVE-float` (Up/Down movement)
- This structure allows users to connect other ESP32 devices for custom applications, while the default configuration is designed for a pre-coded Stewart platform that visualizes the aircraft's movement and orientation.

## Supported Input Devices

**Keyboard & Mouse**:
- Standard input method for accessibility and ease of use.

**Xbox Controller (Wireless/Wired)**:
- Compatible with Xbox controllers for enhanced control and a comfortable experience.

**Flight Simulator Joystick**:
- Support for specialized flight simulator joysticks, enabling a more authentic flying experience (specific models will be determined).

## Getting Started

### Prerequisites
- Unity 2022.3.50f1 or later.
- (Optional) ESP32 microcontroller for Bluetooth/Serial connection with external hardware.
  - Ensure Bluetooth/Serial communication is enabled.

## Usage
- Select the desired game mode from the main menu.
- Use the supported input devices to control the helicopter.
- Adjust settings in the *Options* menu for graphics, controls, and flight sensitivity.
- For a physical representation of the helicopter's movement and orientation, connect a pre-coded ESP32 to the Stewart platform or utilize other ESP32 setups to experiment with the data being sent from the game.