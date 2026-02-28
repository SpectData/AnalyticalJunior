package com.braincademy.junior

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import com.braincademy.junior.navigation.AppNavGraph
import com.braincademy.junior.ui.theme.BrainAcademyTheme

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            BrainAcademyTheme {
                AppNavGraph()
            }
        }
    }
}
