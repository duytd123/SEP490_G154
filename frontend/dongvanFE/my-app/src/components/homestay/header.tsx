"use client"

import { Button } from "../ui/button"
import { Hotel, User, Menu } from "../../lib/icons"
import { Link } from "react-router-dom"
import { useState } from "react"

export default function Header() {
  const [isMenuOpen, setIsMenuOpen] = useState(false)

  return (
    <header className="w-full bg-card border-b border-border sticky top-0 z-50">
      <div className="w-full max-w-screen-2xl mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          {/* Logo */}
          <div className="flex items-center gap-2">
            <Hotel className="h-8 w-8 text-primary" />
            <span className="text-xl font-bold text-foreground">HotelVN</span>
          </div>

          {/* Desktop Navigation */}
          <nav className="hidden md:flex items-center gap-8">
            <Link to="/home" className="text-foreground hover:text-primary transition-colors font-medium">
              Trang chủ
            </Link>
            <Link to="/homestay" className="text-muted-foreground hover:text-primary transition-colors font-medium">
              Homestay
            </Link>
            <a href="#" className="text-muted-foreground hover:text-primary transition-colors font-medium">
              Về chúng tôi
            </a>
            <a href="#" className="text-muted-foreground hover:text-primary transition-colors font-medium">
              Liên hệ
            </a>
          </nav>

          {/* Auth Buttons */}
          <div className="hidden md:flex items-center gap-3">
            <Button variant="ghost" size="sm" className="text-foreground">
              <User className="h-4 w-4 mr-2" />
              Đăng nhập
            </Button>
            <Button variant="primary" size="sm">
              Đăng ký
            </Button>
          </div>

          {/* Mobile Menu Button */}
          <Button variant="ghost" size="sm" className="md:hidden" onClick={() => setIsMenuOpen(!isMenuOpen)}>
            <Menu className="h-5 w-5" />
          </Button>
        </div>

        {/* Mobile Menu */}
        {isMenuOpen && (
          <div className="md:hidden py-4 border-t border-border">
            <nav className="flex flex-col gap-4">
              <Link to="/home" className="text-foreground hover:text-primary transition-colors font-medium">
                Trang chủ
              </Link>
              <Link to="/homestay" className="text-muted-foreground hover:text-primary transition-colors font-medium">
                Homestay
              </Link>
              <a href="#" className="text-muted-foreground hover:text-primary transition-colors font-medium">
                Về chúng tôi
              </a>
              <a href="#" className="text-muted-foreground hover:text-primary transition-colors font-medium">
                Liên hệ
              </a>
              <div className="flex gap-3 pt-2">
                <Button variant="ghost" size="sm" className="text-foreground">
                  <User className="h-4 w-4 mr-2" />
                  Đăng nhập
                </Button>
                <Button variant="primary" size="sm">
                  Đăng ký
                </Button>
              </div>
            </nav>
          </div>
        )}
      </div>
    </header>
  )
}
