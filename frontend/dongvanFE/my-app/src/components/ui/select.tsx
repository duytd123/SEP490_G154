"use client"

export const Select = ({ children, value, onValueChange }: any) => (
  <select value={value} onChange={(e) => onValueChange?.(e.target.value)} className="border rounded px-2 py-1">
    {children}
  </select>
)

export const SelectTrigger = ({ children }: any) => <div>{children}</div>
export const SelectValue = ({ children }: any) => <span>{children}</span>
export const SelectContent = ({ children }: any) => <div>{children}</div>
export const SelectItem = ({ children, value }: any) => <option value={value}>{children}</option>

export default Select
