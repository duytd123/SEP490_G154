"use client"

type ButtonProps = {
  children?: any
  className?: string
  variant?: "primary" | "ghost" | "outline"
  size?: "sm" | "md" | "lg"
  [key: string]: any
}

export const Button = ({ children, className = "", variant = "primary", size = "md", ...props }: ButtonProps) => {
  // adding ripple effect on press
  const base = "relative overflow-hidden inline-flex items-center justify-center gap-2 font-medium transition duration-150 ease-in-out rounded"

  const variants: Record<string, string> = {
    // explicit teal primary so it works regardless of theme tokens
    primary: "bg-teal-600 text-white hover:bg-teal-700 focus:ring-2 focus:ring-teal-300 active:scale-95",
    ghost: "bg-transparent text-foreground hover:bg-muted/50 active:scale-95",
    outline: "bg-transparent border border-border text-foreground hover:bg-muted/10 active:scale-95",
  }

  const sizes: Record<string, string> = {
    sm: "px-2 py-1 text-sm",
    md: "px-4 py-2 text-base",
    lg: "px-6 py-3 text-lg",
  }

  // ripple handler
  const onMouseDown = (e: any) => {
    const btn = e.currentTarget as HTMLElement
    const rect = btn.getBoundingClientRect()
    const ripple = document.createElement("span")
    const size = Math.max(rect.width, rect.height) * 1.5
    ripple.style.position = "absolute"
    ripple.style.left = `${e.clientX - rect.left - size / 2}px`
    ripple.style.top = `${e.clientY - rect.top - size / 2}px`
    ripple.style.width = ripple.style.height = `${size}px`
    ripple.style.background = "rgba(255,255,255,0.3)"
    ripple.style.borderRadius = "50%"
    ripple.style.transform = "scale(0)"
    ripple.style.opacity = "1"
    ripple.style.pointerEvents = "none"
    ripple.style.transition = "transform 400ms ease-out, opacity 400ms ease-out"
    btn.appendChild(ripple)
    requestAnimationFrame(() => {
      ripple.style.transform = "scale(1)"
      ripple.style.opacity = "0"
    })
    setTimeout(() => ripple.remove(), 450)
    // call any onMouseDown passed
    if (props.onMouseDown) props.onMouseDown(e)
  }

  return (
    <button {...props} onMouseDown={onMouseDown} className={`${base} ${variants[variant] ?? variants.primary} ${sizes[size] ?? sizes.md} ${className}`}>
      {children}
    </button>
  )
}

export default Button
