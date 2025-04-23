# DSx.Caching 🚀  
**Un framework di caching avanzato per .NET 9.0**  
![Build Status](https://github.com/Dave2K/DSx.Caching/actions/workflows/ci.yml/badge.svg)
![Coverage](https://img.shields.io/badge/coverage-85%25-green)
![License](https://img.shields.io/badge/license-MIT-blue)

## 📋 Panoramica
Architettura modulare per caching distribuito con:
- Provider Redis e Memory
- Gestione centralizzata delle configurazioni
- Health check integrati
- Supporto per operazioni bulk e cluster

## 🚀 Funzionalità
- **Doppia strategia** di caching (Memory + Redis)
- **Invalidazione intelligente** tramite pattern matching
- **Monitoraggio in tempo reale** con metriche:
  - Hit ratio
  - Tempo medio di accesso
  - Stato del cluster
- **Circuit breaker** integrato per resilienza
- **Gestione centralizzata** delle dipendenze

## 📦 Getting Started
```bash
dotnet add package DSx.Caching.Providers.Redis
dotnet add package DSx.Caching.DependencyInjection