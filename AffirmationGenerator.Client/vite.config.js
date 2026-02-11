import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import tailwindcss from "@tailwindcss/vite";
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';
var target = env.ASPNETCORE_HTTPS_PORT ? "https://localhost:".concat(env.ASPNETCORE_HTTPS_PORT) :
    env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7006';
// https://vitejs.dev/config/
export default defineConfig(function (_a) {
    var command = _a.command;
    var httpsConfig = undefined;
    // Setup https certificates for local development
    if (command === 'serve') {
        var baseFolder = env.APPDATA !== undefined && env.APPDATA !== ''
            ? "".concat(env.APPDATA, "/ASP.NET/https")
            : "".concat(env.HOME, "/.aspnet/https");
        var certificateName = "reactapp1.client";
        var certFilePath = path.join(baseFolder, "".concat(certificateName, ".pem"));
        var keyFilePath = path.join(baseFolder, "".concat(certificateName, ".key"));
        if (!fs.existsSync(baseFolder)) {
            fs.mkdirSync(baseFolder, { recursive: true });
        }
        if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
            if (0 !== child_process.spawnSync('dotnet', [
                'dev-certs',
                'https',
                '--export-path',
                certFilePath,
                '--format',
                'Pem',
                '--no-password',
            ], { stdio: 'inherit', }).status) {
                throw new Error("Could not create certificate.");
            }
        }
        httpsConfig = {
            key: fs.readFileSync(keyFilePath),
            cert: fs.readFileSync(certFilePath),
        };
    }
    return {
        plugins: [tailwindcss(), react()],
        resolve: {
            alias: {
                '@': fileURLToPath(new URL('./src', import.meta.url))
            }
        },
        server: {
            proxy: {
                '^/affirmations': {
                    target: target,
                    secure: false
                },
                '^/swagger': {
                    target: target,
                    secure: false
                },
                '^/openapi': {
                    target: target,
                    secure: false
                }
            },
            port: 5173,
            https: httpsConfig
        }
    };
});
