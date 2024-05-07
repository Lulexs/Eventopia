export interface ItemInterface {
  type: "table" | "corner" | "stage" | "bar" | "image";
  id: string;
  top: number;
  left: number;
  height: number;
  heightFactor: number;
}

export interface TableInterface extends ItemInterface {
  type: "table";
  numberOfSeats: number;
  reserved: boolean;
  price: string;
}

export interface StageInterface extends ItemInterface {
  type: "stage";
}

export interface BarInterface extends ItemInterface {
  type: "bar";
}

export interface CornerInterface extends Partial<ItemInterface> {
  type: "corner";
}

export interface ImageInterface extends ItemInterface {
  type: "image";
}

export interface Line {
  x1: number;
  y1: number;
  x2: number;
  y2: number;
}

export interface SpaceDataType {
  reservedSpaceId: number;
  items: ItemInterface[];
  image: File | null;
  lines: Line[];
  surfaceDimension: { width: number; height: number };
}
