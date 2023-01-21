
export function setItemToLocalStorage<T>(key: string, data: T) {
  localStorage.setItem(key, JSON.stringify(data));
}

export function getItemFromLocalStorage<T>(key: string): T | null {
  const item: string | null = localStorage.getItem(key);
  if(item !== null && item !== "") return JSON.parse(item as string);
  else return null;
}

export function removeItemFromLocalStorage(key: string) {
  localStorage.removeItem(key);
}

export const LOGGED_USER_KEY = 'logged_user_key';