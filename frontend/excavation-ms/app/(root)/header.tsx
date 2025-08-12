'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import {
  LayoutDashboard,
  Truck,
  Users,
  User,
} from 'lucide-react';

const navItems = [
  { name: 'Kontrol Paneli', href: '/', icon: LayoutDashboard },
  { name: 'Araçlar', href: '/vehicles', icon: Truck },
  { name: 'Müşteriler', href: '/customers', icon: Users },
];

export default function Header() {
  const pathname = usePathname();

  return (
    <header className="fixed top-0 left-0 right-0 z-50 bg-zinc-100 shadow-md h-14 flex items-center px-4">
      {/* Title section */}
      <div className="flex-shrink-0 text-lg font-bold">
        Yamaklar Hafriyat
      </div>

      {/* Navigation Items section */}
      <nav className="flex-grow">
        <ul className="flex justify-center space-x-6">
          {navItems.map(({ name, href, icon: Icon }) => {
            const isActive = pathname === href;
            return (
              <li key={name}>
                <Link
                  href={href}
                  className={`flex items-center gap-1 text-sm font-medium px-3 py-2 rounded-md transition-colors ${
                    isActive
                      ? 'text-blue-600 font-semibold border-b-2 border-blue-600'
                      : 'text-gray-600 hover:text-blue-600'
                  }`}
                >
                  <Icon className="h-5 w-5" />
                  {name}
                </Link>
              </li>
            );
          })}
        </ul>
      </nav>

      {/* Profile Section */}
      <div className="ml-auto">
        <Link
            href="/profile"
            className="flex items-center gap-1 text-sm font-medium px-3 py-2 rounded-md text-gray-600 hover:text-blue-600 transition-colors"
        >
            <User className="h-5 w-5" />
            <span>Profil</span>
        </Link>
      </div>
    </header>
  );
}
