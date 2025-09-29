"use client"

import React, { createContext, useContext, useRef, useState, useEffect } from "react"

type PopoverContextType = {
  open: boolean
  setOpen: (v: boolean) => void
}

const PopoverContext = createContext<PopoverContextType | null>(null)

export const Popover = ({ children }: any) => {
  const [open, setOpen] = useState(false)
  const ref = useRef<HTMLDivElement | null>(null)

  // close on outside click
  useEffect(() => {
    const onDoc = (e: MouseEvent) => {
      if (!ref.current) return
      if (!ref.current.contains(e.target as Node)) setOpen(false)
    }
    document.addEventListener("mousedown", onDoc)
    return () => document.removeEventListener("mousedown", onDoc)
  }, [])

  return (
    <PopoverContext.Provider value={{ open, setOpen }}>
      <div ref={ref} className="relative">
        {children}
      </div>
    </PopoverContext.Provider>
  )
}

export const PopoverTrigger = ({ children, asChild = false }: any) => {
  const ctx = useContext(PopoverContext)
  if (!ctx) return null
  const { open, setOpen } = ctx

  const onClick = (e: any) => {
    e?.preventDefault?.()
    setOpen(!open)
  }

  // if asChild, clone the child and attach onClick
  if (asChild && React.isValidElement(children)) {
    const child = children as React.ReactElement<any>
    const originalOnClick = (child.props as any)?.onClick
    return React.cloneElement(child, { onClick: (e: any) => { originalOnClick?.(e); onClick(e) } })
  }

  return (
    <button type="button" onClick={onClick} className="inline-block">
      {children}
    </button>
  )
}

export const PopoverContent = ({ children, className = "", ...props }: any) => {
  const ctx = useContext(PopoverContext)
  if (!ctx) return null
  const { open } = ctx

  if (!open) return null

  return (
    <div {...props} className={`absolute z-50 ${className}`}>
      {children}
    </div>
  )
}

export default Popover
