// Copyright (c) Microsoft Corporation. All rights reserved.
// SPDX-License-Identifier: MIT

// Wifi
#define IOT_CONFIG_WIFI_SSID "<YOUR WIFI NETWORK NAME>"
#define IOT_CONFIG_WIFI_PASSWORD "<YOUR WIFI NETWORK PASSWORD>"

// Azure IoT Configuration
#define IOT_CONFIG_IOTHUB_FQDN "IoT-Project-15989-hub.azure-devices.net"
#define IOT_CONFIG_DEVICE_ID "BMP-SENSOR"
#define IOT_CONFIG_DEVICE_KEY "3hxH1gR77zEe6nRbJVyoEA6KFjRjqcsDChl4ApKmRzM="

// Publish 1 message every 10 minutes
#define TELEMETRY_FREQUENCY_MILLISECS 2000
#define PUSH_VALUE_FREQUECNY (10 * 60 * 1000)
