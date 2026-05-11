# ---------------------------
# BUILD STAGE
# ---------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln .

COPY ["Presentation/PlakaTanima.WebUI/PlakaTanima.WebUI.csproj", "Presentation/PlakaTanima.WebUI/"]
COPY ["Infrastructure/PlakaTanima.Infrastructure/PlakaTanima.Infrastructure.csproj", "Infrastructure/PlakaTanima.Infrastructure/"]
COPY ["Infrastructure/PlakaTanima.Persistence/PlakaTanima.Persistence.csproj", "Infrastructure/PlakaTanima.Persistence/"]
COPY ["Core/PlakaTanima.Application/PlakaTanima.Application.csproj", "Core/PlakaTanima.Application/"]

RUN dotnet restore "Presentation/PlakaTanima.WebUI/PlakaTanima.WebUI.csproj"

COPY . .

RUN dotnet publish "Presentation/PlakaTanima.WebUI/PlakaTanima.WebUI.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# ---------------------------
# RUNTIME STAGE (STABLE)
# ---------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS final
WORKDIR /app

# ---------------------------
# FIXED VERSION SYSTEM PACKAGES
# ---------------------------
RUN apt-get update && apt-get install -y --no-install-recommends \
    gstreamer1.0-tools=1.22.* \
    gstreamer1.0-plugins-base=1.22.* \
    gstreamer1.0-plugins-good=1.22.* \
    libgstreamer1.0-0=1.22.* \
    libgstreamer-plugins-base1.0-0=1.22.* \
    libglib2.0-0 \
    libsm6 \
    libxext6 \
    libxrender1 \
    && rm -rf /var/lib/apt/lists/*

# ---------------------------
# APP COPY
# ---------------------------
COPY --from=build /app/publish .

# OpenCV native runtime
COPY --from=build /src/**/runtimes/linux-x64/native/ /app/runtimes/linux-x64/native/

# ---------------------------
# ENV FIXES
# ---------------------------
ENV LD_LIBRARY_PATH=/app/runtimes/linux-x64/native:$LD_LIBRARY_PATH
ENV GSTREAMER_DEBUG=2
ENV OPENCV_FFMPEG_CAPTURE_OPTIONS=rtsp_transport;tcp
# ASP.NET
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "PlakaTanima.WebUI.dll"]