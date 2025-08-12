export interface IVehicle {
    id: string;
    title: string;
    vehicleType: string;
    photoBase64: string;
}

export interface IVehicleResponse {
  status: string,
  message: string,
  data: IVehicle[]
}

export interface IDeleteVehicleModalProps {
  opened: boolean;
  onClose: () => void;
  vehicleTitle: string | null;
  onConfirm: () => void;
  loading?: boolean;
}

export interface IEditVehicleModalProps {
  opened: boolean;
  onClose: () => void;
  vehicleId: string | null;
  // Add props for vehicle data & update handlers as needed
}
