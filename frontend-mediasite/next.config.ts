import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  images: {
    dangerouslyAllowLocalIP: true,
    remotePatterns: [
      {
        protocol: "https",
        hostname: "localhost",
        port: "7135",
        pathname: "/Uploads/**",
      },
    ],
  },
};

export default nextConfig;
