package com.braincademy.junior.ui.theme

import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable

private val LightColorScheme =
    lightColorScheme(
        primary = Purple60,
        secondary = Purple80,
        background = Background,
        surface = CardBackground,
        onPrimary = CardBackground,
        onSecondary = CardBackground,
        onBackground = TextPrimary,
        onSurface = TextPrimary,
    )

@Composable
fun BrainAcademyTheme(content: @Composable () -> Unit) {
    MaterialTheme(
        colorScheme = LightColorScheme,
        typography = Typography,
        content = content,
    )
}
