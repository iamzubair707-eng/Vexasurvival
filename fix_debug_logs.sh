#!/bin/bash

# Replace Debug.Log with DebugLogger.Log in all C# files
find Scripts/ -name "*.cs" -exec sed -i 's/Debug\.Log(/DebugLogger.Log(/g' {} \;
find Scripts/ -name "*.cs" -exec sed -i 's/Debug\.LogError(/DebugLogger.LogError(/g' {} \;
find Scripts/ -name "*.cs" -exec sed -i 's/Debug\.LogWarning(/DebugLogger.LogWarning(/g' {} \;

echo "✅ All Debug.Log replaced with DebugLogger.Log"
